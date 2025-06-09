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
        private readonly IInterviewRepository _interviewRepo;

        public InterviewsController(IInterviewRepository interviewRepo)
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
    }
}
