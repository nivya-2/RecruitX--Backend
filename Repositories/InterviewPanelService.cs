// File Path: Repositories/InterviewPanelService.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Graph; // This is now the primary tool for Graph calls
using System.Text.Json;
using Microsoft.Graph.Models; // This contains Event, Attendee, etc.

using System.Text.RegularExpressions;

namespace RecruitX.Repositories
{
    public class InterviewPanelService : IInterviewPanelService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<InterviewPanelService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly GraphServiceClient _graphServiceClient; // The SDK client

        public InterviewPanelService(
             AppDbContext context,
             ILogger<InterviewPanelService> logger,
             IConfiguration configuration,
             GraphServiceClient graphServiceClient)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _graphServiceClient = graphServiceClient;
        }


        public async Task<object> TestGraphConnectionAsync()
        {
            var accessToken = await GetAppOnlyTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return new { success = false, message = "Failed to acquire access token." };
            }
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/users?$top=1&$select=displayName");
            var content = await response.Content.ReadAsStringAsync();
            return new
            {
                success = response.IsSuccessStatusCode,
                message = response.IsSuccessStatusCode ? "Graph API connection successful." : "Graph API connection failed.",
                statusCode = (int)response.StatusCode,
                graphResponse = content
            };
        }

        public async Task<IEnumerable<PanelMemberNameAndEmailDto>> GetPanelMembersAsync()
        {
            var accessToken = await GetAppOnlyTokenAsync();
            if (string.IsNullOrEmpty(accessToken)) throw new InvalidOperationException("Failed to obtain access token.");

            var graphResult = await FetchUsersFromGraphAsync(accessToken);
            if (!graphResult.Success) throw new InvalidOperationException("Failed to fetch users from Graph API.");

            var panelMembers = await FilterUsersByPanelRoleAsync(graphResult.Users);
            return panelMembers.OrderBy(p => p.Name);
        }

        public async Task<IEnumerable<PanelMemberDto>> GetPanelMembersWithDetailsAsync()
        {
            var accessToken = await GetAppOnlyTokenAsync();
            if (string.IsNullOrEmpty(accessToken)) throw new InvalidOperationException("Failed to obtain access token.");

            var dbPanelMembers = await _context.Users
                .AsNoTracking()
                .Include(u => u.Employee)
                    .ThenInclude(e => e.Department)
                .Where(u => u.RoleId == 8)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Username,
                    EmployeeName = u.Employee != null ? u.Employee.Name : u.Username,
                    DepartmentName = u.Employee.Department != null ? u.Employee.Department.Name : "N/A"
                }).ToListAsync();

            var graphResult = await FetchUsersFromGraphAsync(accessToken);
            var adUsersDictionary = graphResult.Users?
                                        .Where(u => !string.IsNullOrEmpty(u.Mail))
                                        .ToDictionary(u => u.Mail, u => u, StringComparer.OrdinalIgnoreCase)
                                     ?? new Dictionary<string, GraphUser>();

            return dbPanelMembers.Select(dbUser =>
            {
                adUsersDictionary.TryGetValue(dbUser.Email, out var adUser);
                return new PanelMemberDto
                {
                    Id = dbUser.Id,
                    Username = dbUser.Username,
                    Email = dbUser.Email,
                    DisplayName = adUser?.DisplayName ?? dbUser.EmployeeName,
                    Department = dbUser.DepartmentName,
                    IsActiveInAD = adUser != null
                };
            }).OrderBy(u => u.DisplayName).ToList();
        }

        public async Task<IEnumerable<InterviewMeetingDetailsDto>> GetPanelMemberMeetingsAsync(string email, DateTime date)
        {
            var searchStartDate = date.Date.ToUniversalTime();
            var searchEndDate = searchStartDate.AddDays(1);

            var accessToken = await GetAppOnlyTokenAsync();
            if (string.IsNullOrEmpty(accessToken)) throw new InvalidOperationException("Failed to obtain access token.");

            return await GetUserMeetingsAsync(accessToken, email, searchStartDate, searchEndDate);
        }

        public async Task<ScheduleInterviewResponseDto> ScheduleInterviewAsync(ScheduleInterviewRequestDto request, string organizerEmail)
        {
            var accessToken = await GetAppOnlyTokenAsync();
            if (string.IsNullOrEmpty(accessToken)) throw new InvalidOperationException("Failed to obtain access token.");

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
                _logger.LogError(ex, "Failed to parse date/time strings.");
                throw new FormatException("Invalid date or time format.");
            }

            var subject = $"{request.JobRole} Interview ({request.InterviewLevel}) - {request.CandidateName}";
            var newEvent = new
            {
                subject = subject,
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
                _logger.LogError("Failed to schedule interview. Status: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);
                throw new HttpRequestException($"Failed to create event in Graph. Details: {responseContent}", null, response.StatusCode);
            }

            var createdEvent = JsonSerializer.Deserialize<CalendarEvent>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return new ScheduleInterviewResponseDto
            {
                Id = createdEvent.Id,
                Subject = createdEvent.Subject,
                TeamsJoinUrl = createdEvent.OnlineMeeting?.JoinUrl,
                WebLink = createdEvent.WebLink,
                StartTime = createdEvent.Start.DateTime,
                EndTime = createdEvent.End.DateTime
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

        private async Task<GraphApiResult> FetchUsersFromGraphAsync(string accessToken)
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
                    graphApiUrl = GetNextLink(responseContent);
                }
                result.Users = allUsers;
                result.Success = true;
            }
            catch (Exception ex) { result.ErrorMessage = ex.Message; }
            return result;
        }

        // CORRECTED: This method now correctly accepts the internal GraphUser DTO
        private async Task<List<PanelMemberNameAndEmailDto>> FilterUsersByPanelRoleAsync(List<GraphUser> adUsers)
        {
            var dbPanelEmailList = await _context.Users
                .Where(u => u.RoleId == 8 && !string.IsNullOrEmpty(u.Email))
                .Select(u => u.Email)
                .ToListAsync();

            var dbPanelEmails = new HashSet<string>(dbPanelEmailList, StringComparer.OrdinalIgnoreCase);

            return adUsers
                .Where(adUser => !string.IsNullOrEmpty(adUser.Mail) && dbPanelEmails.Contains(adUser.Mail))
                .Select(adUser => new PanelMemberNameAndEmailDto
                {
                    Name = adUser.DisplayName,
                    Email = adUser.Mail
                })
                .ToList();
        }

        private async Task<List<InterviewMeetingDetailsDto>> GetUserMeetingsAsync(string accessToken, string userEmail, DateTime startDate, DateTime endDate)
        {
            var interviews = new List<InterviewMeetingDetailsDto>();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var graphApiUrl = $"https://graph.microsoft.com/v1.0/users/{userEmail}/calendarView?startDateTime={startDate:o}&endDateTime={endDate:o}&$select=id,subject,start,end,organizer,attendees,onlineMeeting,isOnlineMeeting,webLink&$orderby=start/dateTime";
            
            var response = await _httpClient.GetAsync(graphApiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var eventsResponse = JsonSerializer.Deserialize<CalendarEventsResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (eventsResponse?.Value != null)
                {
                    foreach (var evt in eventsResponse.Value.Where(e => e.IsOnlineMeeting == true))
                    {
                        interviews.Add(ParseEventToInterviewDetails(evt));
                    }
                }
            }
            else { _logger.LogWarning("Failed to get meetings for {userEmail}: {statusCode}", userEmail, response.StatusCode); }
            
            return interviews;
        }

        private InterviewMeetingDetailsDto ParseEventToInterviewDetails(CalendarEvent evt)
        {
            var details = new InterviewMeetingDetailsDto
            {
                Id = evt.Id, Subject = evt.Subject,
                StartTime = DateTime.Parse(evt.Start.DateTime, null, DateTimeStyles.RoundtripKind),
                EndTime = DateTime.Parse(evt.End.DateTime, null, DateTimeStyles.RoundtripKind),
                TimeZone = evt.Start.TimeZone, TeamsJoinUrl = evt.OnlineMeeting?.JoinUrl,
                Organizer = evt.Organizer?.EmailAddress?.Name,
                Attendees = evt.Attendees?.Select(a => new MeetingAttendeeDto { Name = a.EmailAddress?.Name, Email = a.EmailAddress?.Address }).ToList() ?? new List<MeetingAttendeeDto>()
            };
            var match = Regex.Match(evt.Subject, @"Interview[s]?[:\s]+(?<candidate>.*?)(\s+for\s+)(?<position>.*)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                details.CandidateName = match.Groups["candidate"].Value.Trim();
                details.Position = match.Groups["position"].Value.Trim();
            } else {
                details.CandidateName = "N/A";
                details.Position = "N/A";
            }
            return details;
        }

        private string GetNextLink(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                return doc.RootElement.TryGetProperty("@odata.nextLink", out var nextLinkElement) ? nextLinkElement.GetString() : null;
            }
            catch { return null; }
        }
        #endregion

        #region Internal DTOs for Graph API Deserialization
        private class GraphUsersResponse { public List<GraphUser> Value { get; set; } }
        private class GraphUser { public string DisplayName { get; set; } public string UserPrincipalName { get; set; } public string Mail { get; set; } }
        private class GraphApiResult { public bool Success { get; set; } public List<GraphUser> Users { get; set; } = new(); public string ErrorMessage { get; set; } }
        private class CalendarEventsResponse { public List<CalendarEvent> Value { get; set; } }
        private class CalendarEvent { public string Id { get; set; } public string Subject { get; set; } public string WebLink { get; set; } public EventDateTime Start { get; set; } public EventDateTime End { get; set; } public Organizer Organizer { get; set; } public List<Attendee> Attendees { get; set; } = new(); public OnlineMeetingInfo OnlineMeeting { get; set; } public bool? IsOnlineMeeting { get; set; } }
        private class EventDateTime { public string DateTime { get; set; } public string TimeZone { get; set; } }
        private class Organizer { public EmailAddress EmailAddress { get; set; } }
        private class Attendee { public EmailAddress EmailAddress { get; set; } }
        private class EmailAddress { public string Name { get; set; } public string Address { get; set; } }
        private class OnlineMeetingInfo { public string JoinUrl { get; set; } }
        #endregion
    }
}