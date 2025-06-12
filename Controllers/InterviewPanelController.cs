using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Secures all endpoints in this controller
    public class InterviewPanelController : ControllerBase
    {
        private readonly IInterviewPanelService _panelService;
        private readonly ILogger<InterviewPanelController> _logger;

        public InterviewPanelController(IInterviewPanelService panelService, ILogger<InterviewPanelController> logger)
        {
            _panelService = panelService;
            _logger = logger;
        }

        [HttpGet("panel-members")]
        public async Task<IActionResult> GetPanelMembers()
        {
            try
            {
                var panelMembers = await _panelService.GetPanelMembersAsync();
                return Ok(panelMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching panel members.");
                return StatusCode(500, new { message = "An internal server error occurred while fetching panel members." });
            }
        }

        [HttpGet("panel-meetings")]
        public async Task<IActionResult> GetPanelMemberMeetings([FromQuery, Required] string email, [FromQuery, Required] string date)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(date))
            {
                return BadRequest("Email and date are required.");
            }

            if (!DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return BadRequest("Invalid date format. Please use 'dd-MM-yyyy'.");
            }

            try
            {
                var meetings = await _panelService.GetPanelMemberMeetingsAsync(email, parsedDate);
                // We return the meetings directly. The frontend will know which email they belong to.
                return Ok(meetings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching meetings for {email}", email);
                return StatusCode(500, new { message = "An error occurred." });
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
                var organizerEmail = User.FindFirst(ClaimTypes.Email)?.Value
                                  ?? User.FindFirst("preferred_username")?.Value;

                if (string.IsNullOrEmpty(organizerEmail))
                {
                    // Fallback for testing without a real login token
                    organizerEmail = "abhinandk@RecruitXexp.onmicrosoft.com";
                    _logger.LogWarning("User email not found in claims. Using default organizer: {organizerEmail}", organizerEmail);
                }

                var result = await _panelService.ScheduleInterviewAsync(request, organizerEmail);

                // Return 201 Created status with the new object in the body
                return CreatedAtAction(nameof(GetPanelMemberMeetings), result);
            }
            catch (FormatException ex)
            {
                return BadRequest(new { message = "Data formatting error.", details = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Graph API error while scheduling interview.");
                return StatusCode(500, new { message = "An error occurred with Microsoft Graph.", details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while scheduling an interview.");
                return StatusCode(500, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }
    }
}