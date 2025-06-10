using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitX.Data;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;
using static RecruitX.Controllers.ApplicationDetailsDTO;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobDescriptionController : ControllerBase
    {
        private readonly IJobDescriptionService _jobTrackingService;
        private readonly ILogger<JobDescriptionController> _logger;
        private readonly IRecruitmentEmailService _recruitmentEmailService;

        private readonly AppDbContext _context;
        private static readonly List<ApplicationStatus> Workflow = new List<ApplicationStatus>
    {
        ApplicationStatus.Applied,
        ApplicationStatus.TechnicalInterview,
        ApplicationStatus.ManagementInterview,
        ApplicationStatus.DocumentationVerified,
        ApplicationStatus.SalaryApproved,
        ApplicationStatus.OfferLetterAccepted,
        ApplicationStatus.Joined
    };


        public JobDescriptionController(IJobDescriptionService jobTrackingService, ILogger<JobDescriptionController> logger, AppDbContext context, IRecruitmentEmailService recruitmentEmailService)
        {
            _jobTrackingService = jobTrackingService;
            _logger = logger;
            _recruitmentEmailService = recruitmentEmailService;

            _context = context;
        }
        [HttpGet("draft/{jobRequisitionId:int}")]
        public async Task<IActionResult> Generate(int jobRequisitionId)
        {
            var dto = await _jobTrackingService.GenerateJobDescriptionFromRequisitionAsync(jobRequisitionId);
            if (dto == null)
                return NotFound($"Job Requisition with ID {jobRequisitionId} not found.");

            return Ok(dto);
        }

        [HttpPost("save-draft")]
        public async Task<IActionResult> SaveDraftJobDescription([FromBody] JobDescriptionDTO dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                                            ?? User.FindFirst("preferred_username")?.Value; if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var result = await _jobTrackingService.SaveDraftJobDescriptionAsync(dto, userEmail);
            if (!result)
                return BadRequest("Invalid Job Requisition ID or user not found.");

            return Ok("Job description saved as draft.");
        }

        [HttpPut("generateJD")]
        public async Task<IActionResult> SubmitJobDescription([FromBody] JobDescriptionDTO dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                                            ?? User.FindFirst("preferred_username")?.Value; if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var result = await _jobTrackingService.SubmitJobDescriptionAsync(dto, userEmail);
            if (!result)
                return BadRequest("Either JD was not drafted or Job Requisition is invalid.");

            return Ok("Job description submitted.");
        }

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


        [HttpGet("my-job-descriptions/{id:int}")] // e.g., GET /api/job-descriptions/123
        public async Task<ActionResult<JobDescriptionDTO>> GetJobDescriptionDetail(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve details for Job Requisition ID: {JobRequisitionId}", id);

                var jobDetailsDto = await _jobTrackingService.GetJobDescriptionDetailsAsync(id);

                if (jobDetailsDto == null)
                {
                    _logger.LogWarning("Job Requisition with ID {JobRequisitionId} not found.", id);
                    return NotFound($"No job description found for ID {id}.");
                }

                _logger.LogInformation("Successfully retrieved details for Job Requisition ID: {JobRequisitionId}", id);
                return Ok(jobDetailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving details for Job Requisition ID: {JobRequisitionId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the job details.");
            }
        }



        [HttpGet("my-job-descriptions/applicants/{requisitionIdentifier:int}")] // Path parameter is now the business JR ID
      
        public async Task<IActionResult> GetApplicantsForJd(int requisitionIdentifier) // Parameter is string, adjust if JR ID is int
        {
            // Find the JobDescription by its business RequisitionIdentifier
            // Assumes JobDescription entity has a 'RequisitionIdentifier' property.
            // Adjust 'jd.RequisitionIdentifier' to your actual property name.
            var jobDescription = await _context.JobDescriptions
                                        .FirstOrDefaultAsync(jd => jd.JobRequisitionId == requisitionIdentifier);

            if (jobDescription == null)
            {
                return NotFound($"Job Requisition with JR ID '{requisitionIdentifier}' not found.");
            }

            // Now we have the jobDescription.Id (which is the PK), pass it to the service
            var applicants = await _jobTrackingService.GetApplicantsForJdAsync(jobDescription.Id);

            return Ok(applicants ?? new List<JdApplicantsDTO>());
        }

        [HttpGet("my-job-descriptions/applicant-details/{applicationId}")]
        public async Task<ActionResult<ApplicationDetailsPageDTO>> GetApplicationPageDetails(int applicationId)
        {
            var result = await _jobTrackingService.GetApplicationPageDetailsAsync(applicationId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok (result );
        }

        [HttpPost("my-job-descriptions/applicant-details/{id}/update-status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequestDto request)
        {
            var success = await _jobTrackingService.UpdateApplicationStatusAsync(id, request.Action);
            if (!success)
            {
                return BadRequest("Failed to update status. The application may be in a finished state or the action is invalid.");
            }
            return Ok();
        }

        [HttpPost("my-job-descriptions/{jobRequisitionId:int}/bulk-add")]
        public async Task<ActionResult<BulkAddResultDTO>> BulkAddCandidates(
       int jobRequisitionId,
       [FromBody] List<CandidateDetailsDTO> candidates)
        {
            // --- THIS IS THE CHANGE ---
            // Get the current user's email from their authentication token (claim).
            // Using FindFirstValue is a convenient shortcut.
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                // Also check for 'preferred_username' as a fallback, common in some OIDC providers
                userEmail = User.FindFirstValue("preferred_username");
            }

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return Unauthorized("User email claim is missing or invalid.");
            }

            if (candidates == null || !candidates.Any())
            {
                return BadRequest("Candidate list cannot be empty.");
            }

            // Pass the userEmail string to the service
            var result = await _jobTrackingService.BulkAddCandidatesAsync(jobRequisitionId, candidates, userEmail);
            try
            {
                var screeningEmailTasks = result.SuccessfulApplicationIds
                    .Select(appId => _recruitmentEmailService.SendScreeningEmailAsync(appId));

                await Task.WhenAll(screeningEmailTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending screening emails after bulk add.");
                // Optional: Add to FailureMessages or handle however needed
            }

            // The response logic remains the same
            if (result.SuccessCount > 0 && result.FailureCount > 0)
            {
                return StatusCode(207, result);
            }
            if (result.FailureCount > 0)
            {
                return BadRequest(result);
            }

            return Created("", result);
        }

    }
}
