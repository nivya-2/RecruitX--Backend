using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _interviewRepo;

        public InterviewsController(IInterviewService interviewRepo)
        {
            _interviewRepo = interviewRepo;
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

        [HttpGet("schedule/{jrId}")]
        public async Task<IActionResult> GetCandidatesByJD(int jrId)
        {
            var candidates = await _interviewRepo.GetCandidatesByJobDescriptionIdAsync(jrId);
            return Ok(candidates);
        }

        [HttpPost("shortlist")]

        public async Task<IActionResult> ShortlistCandidate(int jrId, int CanId)
        {
            var success = await _interviewRepo.ShortlistCandidateAsync(jrId, CanId);
            if (!success)
            {
                return BadRequest("Application not found or is not in a state that can be shortlisted.");
            }
            return NoContent();
        }

        [HttpPost("reject")]

        public async Task<IActionResult> RejectCandidate(int jrId, int CanId)
        {
            var success = await _interviewRepo.RejectCandidateAsync(jrId, CanId);
            if (!success)
            {
                return BadRequest("Application not found or is already in a terminal state.");
            }
            return NoContent();
        }

        [HttpPost("add-rounds")]
      
        public async Task<IActionResult> IncrementInterviewCount(int jrId, int CanId)
        {
            var success = await _interviewRepo.IncrementInterviewCountAsync(jrId, CanId);

            if (!success)
            {
                // This indicates that no matching application was found for the given JR and Candidate.
                return NotFound("No matching application found for the specified Job Requisition and Candidate.");
            }

            return NoContent(); // A 204 No Content is a standard and appropriate response for a successful update with no body.
        }
    }
}
