// File Path: Controllers/InterviewPanelController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims; // ADDED: Required for accessing user claims like email

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class InterviewPanelController : ControllerBase
    {
        private readonly IInterviewPanelService _panelService;
        private readonly ILogger<InterviewPanelController> _logger;

        public InterviewPanelController(IInterviewPanelService panelService, ILogger<InterviewPanelController> logger)
        {
            _panelService = panelService;
            _logger = logger;
        }

        [HttpGet("test-graph-connection")]
        public async Task<IActionResult> TestGraphConnection()
        {
            try
            {
                var result = await _panelService.TestGraphConnectionAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during graph connection test.");
                return StatusCode(500, new { success = false, message = "An internal server error occurred.", error = ex.Message });
            }
        }

        [HttpGet("panel-members")]
        public async Task<IActionResult> GetPanelMembers()
        {
            try
            {
                var panelMembers = await _panelService.GetPanelMembersAsync();

                // Return the list of strings directly.
                // The frontend will receive a simple JSON array: ["Alan Turing", "Jessica Brown"]
                return Ok(panelMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching panel members.");
                // In case of an error, we still return a structured error object.
                return StatusCode(500, new { success = false, message = "An internal server error occurred.", error = ex.Message });
            }
        }

        // ... (all other methods like GetPanelMembersWithDetails, etc. are unchanged) ...
    




[HttpGet("panel-meetings")]
        public async Task<IActionResult> GetPanelMemberMeetings(
         [FromQuery, Required] string email,
         [FromQuery, Required] string date) // Changed from DateTime? to string
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Both 'email' and 'date' query parameters are required." });
            }

            // Step 1: Parse the incoming string date
            DateTime parsedDate;
            try
            {
                // Use ParseExact to enforce the "dd-MM-yyyy" format.
                parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                // If parsing fails, return a clear error message.
                return BadRequest(new { success = false, message = "Invalid date format. Please use 'dd-MM-yyyy'." });
            }

            // Step 2: Proceed with the parsed DateTime object
            try
            {
                // The service layer still receives a proper DateTime object, so it doesn't need to change.
                var meetings = await _panelService.GetPanelMemberMeetingsAsync(email, parsedDate);

                return Ok(new
                {
                    success = true,
                    // Use the original string date in the response message for consistency.
                    message = $"Found {meetings.Count()} interview(s) for {email} on {date}.",
                    searchParameters = new { panelMemberEmail = email, searchDate = date },
                    interviews = meetings
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching meetings for {Email}", email);
                return StatusCode(500, new { success = false, message = "An internal server error occurred.", error = ex.Message });
            }
        }

        [HttpPost("schedule-interview")]
        public async Task<IActionResult> ScheduleInterview([FromBody] ScheduleInterviewRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // --- START OF MODIFIED ORGANIZER LOGIC ---

                // Try to get the organizer's email from the logged-in user's token.
                var organizerEmail = User.FindFirst(ClaimTypes.Email)?.Value
                                  ?? User.FindFirst("preferred_username")?.Value;

                // If no token is present (e.g., testing from Swagger/Postman without auth),
                // set a default organizer email.
                if (string.IsNullOrEmpty(organizerEmail))
                {
                    organizerEmail = "abhinandk@RecruitXexp.onmicrosoft.com"; // Your requested default email
                    _logger.LogWarning("No authenticated user found. Using default organizer email: {DefaultEmail}", organizerEmail);
                }

                // --- END OF MODIFIED ORGANIZER LOGIC ---

                // Call the service, passing both the request DTO and the determined organizer's email.
                var result = await _panelService.ScheduleInterviewAsync(request, organizerEmail);

                return Created(string.Empty, result);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid date/time format received.");
                return BadRequest(new { message = "Date/time format error.", details = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Graph API error while scheduling interview.");
                return StatusCode((int)ex.StatusCode, new { message = "An error occurred with Microsoft Graph.", details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while scheduling an interview.");
                return StatusCode(500, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }
    }
}
