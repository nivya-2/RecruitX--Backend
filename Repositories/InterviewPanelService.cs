// File Path: Repositories/InterviewPanelService.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RecruitX.Repositories
{
    public class InterviewPanelService : IInterviewPanelService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<InterviewPanelService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public InterviewPanelService(
            AppDbContext context,
            ILogger<InterviewPanelService> logger,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        // ===================================================================
        // THIS IS THE CORRECTED METHOD
        // ===================================================================
        public async Task<IEnumerable<PanelMemberNameAndEmailDto>> GetPanelMembersAsync()
        {
            var accessToken = await GetAppOnlyTokenAsync();
            var graphResult = await FetchAllUsersFromGraphAsync(accessToken);

            if (!graphResult.Success)
            {
                throw new InvalidOperationException("Failed to fetch users from Graph API.");
            }

            // --- THIS IS THE LINE THAT WAS MISSING ---
            var dbPanelEmailList = await _context.Users
                .Where(u => u.RoleId == 8 && !string.IsNullOrEmpty(u.Email))
                .Select(u => u.Email)
                .ToListAsync();
            // --- END OF MISSING LINE ---

            var dbPanelEmails = new HashSet<string>(dbPanelEmailList, StringComparer.OrdinalIgnoreCase);

            var panelMembers = graphResult.Users
                .Where(adUser => !string.IsNullOrEmpty(adUser.Mail) && dbPanelEmails.Contains(adUser.Mail))
                .Select(adUser => new PanelMemberNameAndEmailDto
                {
                    Name = adUser.DisplayName,
                    Email = adUser.Mail
                })
                .OrderBy(p => p.Name)
                .ToList();

            return panelMembers;
        }

        public async Task<IEnumerable<InterviewMeetingDetailsDto>> GetPanelMemberMeetingsAsync(string email, DateTime date)
        {
            var accessToken = await GetAppOnlyTokenAsync();
            var searchStartDate = date.Date.ToUniversalTime();
            var searchEndDate = searchStartDate.AddDays(1);


            return await GetMeetingsForSingleUserAsync(accessToken, email, searchStartDate, searchEndDate);
        
        }

        public async Task<ScheduleInterviewResponseDto> ScheduleInterviewAsync(ScheduleInterviewRequestDto request, string organizerEmail)
        {
            var accessToken = await GetAppOnlyTokenAsync();
            DateTime localStartTime;
            DateTime localEndTime;

            try
            {
                string startDateTimeString = $"{request.Date} {request.StartTime}";
                string endDateTimeString = $"{request.Date} {request.EndTime}";
                string format = "dd-MM-yyyy h:mm tt";
                localStartTime = DateTime.ParseExact(startDateTimeString, format, CultureInfo.InvariantCulture);
                localEndTime = DateTime.ParseExact(endDateTimeString, format, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse date/time strings during scheduling.");
                throw new FormatException("Invalid date or time format provided.");
            }

            var newEvent = new
            {
                subject = $"{request.JobRole} Interview ({request.InterviewLevel}) - {request.CandidateName}",
                body = new { contentType = "HTML", content = $"Interview for <b>{request.CandidateName}</b>." },
                start = new { dateTime = localStartTime.ToString("yyyy-MM-ddTHH:mm:ss"), timeZone = "India Standard Time" },
                end = new { dateTime = localEndTime.ToString("yyyy-MM-ddTHH:mm:ss"), timeZone = "India Standard Time" },
                attendees = request.PanelMemberEmails.Select(e => new { emailAddress = new { address = e, name = e }, type = "required" }).ToList(),
                isOnlineMeeting = true,
                onlineMeetingProvider = "teamsForBusiness"
            };

            var jsonPayload = JsonSerializer.Serialize(newEvent);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var graphApiUrl = $"https://graph.microsoft.com/v1.0/users/{organizerEmail}/events?sendInvitations=false";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsync(graphApiUrl, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to schedule interview via Graph API. Status: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);
                throw new HttpRequestException($"Failed to create event in Graph. Details: {responseContent}", null, response.StatusCode);
            }

            var createdEvent = JsonSerializer.Deserialize<CalendarEvent>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return new ScheduleInterviewResponseDto
            {
                Id = createdEvent.Id,
                Subject = createdEvent.Subject,
                TeamsJoinUrl = createdEvent.OnlineMeeting?.JoinUrl,
                WebLink = createdEvent.WebLink
            };
        }

        #region Private Helper Methods

        private async Task<string> GetAppOnlyTokenAsync()
        {
            try
            {
                var app = ConfidentialClientApplicationBuilder
                    .Create(_configuration["AzureAd:ClientId"])
                    .WithClientSecret(_configuration["AzureAd:ClientSecret"])
                    .WithAuthority(new Uri($"https://login.microsoftonline.com/{_configuration["AzureAd:TenantId"]}"))
                    .Build();
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                return result.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to acquire application-only access token.");
                throw;
            }
        }

        private async Task<GraphApiResult> FetchAllUsersFromGraphAsync(string accessToken)
        {
            var result = new GraphApiResult();
            var allUsers = new List<GraphUser>();
            var graphApiUrl = "https://graph.microsoft.com/v1.0/users?$select=displayName,userPrincipalName,mail&$top=999";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                while (!string.IsNullOrEmpty(graphApiUrl))
                {
                    var response = await _httpClient.GetAsync(graphApiUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        result.ErrorMessage = $"Graph API call failed: {response.StatusCode}";
                        return result;
                    }
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var graphResponse = JsonSerializer.Deserialize<GraphUsersResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (graphResponse?.Value != null) allUsers.AddRange(graphResponse.Value);

                    using var doc = JsonDocument.Parse(responseContent);
                    graphApiUrl = doc.RootElement.TryGetProperty("@odata.nextLink", out var nextLinkElement) ? nextLinkElement.GetString() : null;
                }
                result.Users = allUsers;
                result.Success = true;
            }
            catch (Exception ex) { result.ErrorMessage = ex.Message; }
            return result;
        }

        private async Task<List<InterviewMeetingDetailsDto>> GetMeetingsForSingleUserAsync(string accessToken, string userEmail, DateTime startDate, DateTime endDate)
        {
            var meetings = new List<InterviewMeetingDetailsDto>();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var graphApiUrl = $"https://graph.microsoft.com/v1.0/users/{userEmail}/calendarView?startDateTime={startDate:o}&endDateTime={endDate:o}&$select=id,subject,start,end,attendees&$orderby=start/dateTime";

            var response = await _httpClient.GetAsync(graphApiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var eventsResponse = JsonSerializer.Deserialize<CalendarEventsResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (eventsResponse?.Value != null)
                {
                    foreach (var evt in eventsResponse.Value)
                    {
                        meetings.Add(ParseEventToDto(evt, userEmail));
                    }
                }
            }
            else { _logger.LogWarning("Failed to get meetings for {userEmail}: {statusCode}", userEmail, response.StatusCode); }

            return meetings;
        }

        private InterviewMeetingDetailsDto ParseEventToDto(CalendarEvent evt, string primaryInterviewerEmail)
        {
            var startDate = DateTime.Parse(evt.Start.DateTime, null, DateTimeStyles.RoundtripKind);
            var endDate = DateTime.Parse(evt.End.DateTime, null, DateTimeStyles.RoundtripKind);

            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var istStartTime = TimeZoneInfo.ConvertTimeFromUtc(startDate, istZone);
            var istEndTime = TimeZoneInfo.ConvertTimeFromUtc(endDate, istZone);

            var candidateNameMatch = Regex.Match(evt.Subject, @"Interview: (.*?) for");

            return new InterviewMeetingDetailsDto
            {
                Id = evt.Id,
                Subject = evt.Subject,
                CandidateName = candidateNameMatch.Success ? candidateNameMatch.Groups[1].Value.Trim() : "External Meeting",
                StartTime = istStartTime.ToString("HH:mm"),
                EndTime = istEndTime.ToString("HH:mm"),
                InterviewerEmails = new List<string> { primaryInterviewerEmail }
            };
        }

        #endregion

        #region Internal DTOs for Deserialization
        private class GraphApiResult { public bool Success { get; set; } public List<GraphUser> Users { get; set; } = new(); public string ErrorMessage { get; set; } }
        private class GraphUsersResponse { public List<GraphUser> Value { get; set; } }
        private class GraphUser { public string DisplayName { get; set; } public string UserPrincipalName { get; set; } public string Mail { get; set; } }
        private class CalendarEventsResponse { public List<CalendarEvent> Value { get; set; } }
        private class CalendarEvent { public string Id { get; set; } public string Subject { get; set; } public string WebLink { get; set; } public EventDateTime Start { get; set; } public EventDateTime End { get; set; } public List<Attendee> Attendees { get; set; } = new(); public OnlineMeetingInfo OnlineMeeting { get; set; } }
        private class EventDateTime { public string DateTime { get; set; } public string TimeZone { get; set; } }
        private class Attendee { public EmailAddress EmailAddress { get; set; } }
        private class EmailAddress { public string Name { get; set; } public string Address { get; set; } }
        private class OnlineMeetingInfo { public string JoinUrl { get; set; } }
        #endregion
    }
}