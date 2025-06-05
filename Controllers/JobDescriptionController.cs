using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobDescriptionController : ControllerBase
    {
        private readonly ITrackJdService _jobTrackingService;
        private readonly ILogger<JobDescriptionController> _logger;

        public JobDescriptionController(ITrackJdService jobTrackingService, ILogger<JobDescriptionController> logger)
        {
            _jobTrackingService = jobTrackingService;
            _logger = logger;
        }

        // [Authorize] // Uncomment when using authentication
        [HttpGet("my-job-descriptions")]
        public async Task<ActionResult<IEnumerable<TrackJdDTO>>> GetMyJobDescriptions()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                                ?? User.FindFirst("preferred_username")?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("User email claim not found for authorized user.");
                    return Unauthorized("User email not found in claims. Unable to fetch job descriptions.");
                }

                _logger.LogInformation("Retrieving generated job descriptions for user: {UserEmail}", userEmail);
                var jobDescriptions = await _jobTrackingService.GetJobDescriptionsForUserAsync(userEmail);

                return Ok(jobDescriptions); // Empty list returns 200 OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job descriptions for user.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving your job descriptions.");
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<PendingJdDTO>>> GetMyPendingJds()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                                ?? User.FindFirst("preferred_username")?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("User email claim not found for authorized user.");
                    return Unauthorized("User email not found in claims. Unable to fetch pending JDs.");
                }

                _logger.LogInformation("Retrieving pending job descriptions for user: {UserEmail}", userEmail);
                var pendingJds = await _jobTrackingService.GetPendingJdsForUserAsync(userEmail);

                return Ok(pendingJds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending job descriptions for user.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving your pending job descriptions.");
            }
        }
    }
}
