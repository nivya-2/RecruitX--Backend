using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // NOTE: No [Authorize] attribute. This controller is for public access.
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;
        private readonly ILogger<EvaluationController> _logger;

        public EvaluationController(IEvaluationService evaluationService, ILogger<EvaluationController> logger)
        {
            _evaluationService = evaluationService;
            _logger = logger;
        }

        /// <summary>
        /// Validates an evaluation token and retrieves form header details.
        /// This is called when the interviewer first opens the one-time link.
        /// e.g., GET /api/evaluation/abcd-1234-efgh-5678
        /// </summary>
        /// <param name="token">The unique token from the URL.</param>
        [HttpGet("{token}")]
        public async Task<IActionResult> GetFormDetailsByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Evaluation token is required." });
            }

            try
            {
                var formDetails = await _evaluationService.GetEvaluationFormDetailsAsync(token);

                if (formDetails == null)
                {
                    // This means the token is invalid, expired, or already used.
                    // We use 404 Not Found, which is appropriate for a link that no longer works.
                    return NotFound(new { message = "This evaluation link is invalid or has already been submitted." });
                }

                return Ok(formDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating evaluation token {Token}", token);
                return StatusCode(500, new { message = "An error occurred while validating the link." });
            }
        }
        [HttpGet("view/{interviewId:int}")]
        [Authorize] // This specific endpoint is protected.
        public async Task<IActionResult> GetSubmittedEvaluation(int interviewId)
        {
            try
            {
                var evaluation = await _evaluationService.GetSubmittedEvaluationAsync(interviewId);

                if (evaluation == null)
                {
                    // This means either no link was ever created for this interview,
                    // or the form has not been submitted yet.
                    return NotFound(new { message = $"No submitted evaluation found for interview ID {interviewId}." });
                }

                return Ok(evaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving submitted evaluation for interview ID {InterviewId}", interviewId);
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        /// <summary>
        /// Submits the completed evaluation form.
        /// e.g., POST /api/evaluation/submit
        /// </summary>
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitEvaluation([FromBody] SubmitEvaluationDto submissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _evaluationService.SubmitEvaluationAsync(submissionDto);

                if (!success)
                {
                    // This happens if the token became invalid between page load and submission.
                    return BadRequest(new { message = "This evaluation link is invalid or has already been submitted." });
                }

                return Ok(new { message = "Thank you, your feedback has been submitted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting evaluation for token {Token}", submissionDto.Token);
                return StatusCode(500, new { message = "An error occurred while submitting your feedback." });
            }
        }
    }
}