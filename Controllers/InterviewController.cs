using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;
using RecruitX.Repositories;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _interviewRepo;
        private readonly IEvaluationService _evaluationService;


        public InterviewsController(IInterviewService interviewRepo, IEvaluationService evaluationService)
        {
            _interviewRepo = interviewRepo;
            _evaluationService = evaluationService;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InterviewDTO>>> GetInterviews()
        {
            var interviews = await _interviewRepo.GetAllInterviewsAsync();
            return Ok(interviews);
        }
        [HttpGet("to-schedule")]
        public async Task<ActionResult<IEnumerable<ToScheduleDto>>> GetToSchedule()
        {
            var result = await _interviewRepo.GetToScheduleInterviewsAsync();
            return Ok(result);
        }
        [HttpGet("to-shortlist")]
        public async Task<ActionResult<IEnumerable<ToShortlistDto>>> GetToShortlist()
        {
            var result = await _interviewRepo.GetInterviewsToShortlistAsync();
            return Ok(result);
        }

        [HttpGet("generate-test-link/{interviewId:int}")]
        public async Task<IActionResult> GenerateTestLink(int interviewId)
        {
            try
            {
                // This reuses the exact same logic your real application will use.
                var linkUrl = await _evaluationService.CreateEvaluationLinkAsync(interviewId);

                //_logger.LogWarning("Generated a TEST evaluation link: {Link}", linkUrl);

                // Return the link as plain text for easy copying.
                return Content(linkUrl, "text/plain");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Could not generate test link for interview ID {InterviewId}", interviewId);
                return StatusCode(500, new { message = "Failed to generate test link. Does the interview ID exist?" });
            }
        }
    }
}
