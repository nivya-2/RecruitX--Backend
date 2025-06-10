using System.Security.Claims;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;
using RecruitX.Repositories;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAssignedJrService _assignedService;
        private readonly ITeamService _teamService;

        public UsersController(IAssignedJrService assignedService, ITeamService teamService)
        {
            _assignedService = assignedService;
            _teamService = teamService;
        }

        [HttpGet("{userId}/job-requisitions")]
        public async Task<IActionResult> GetAssignedJobRequisitions(int userId)
        {
            var result = await _assignedService.GetAssignedJobRequisitionsByUserNameAsync(userId);

            return Ok(result);
        }

        [HttpGet("all-team")]
        public async Task<IActionResult> GetTeamMembers()
        {
            var team = await _teamService.GetTeamMembersAsync();
            return Ok(team); // Will not include UserId due to [JsonIgnore]
        }

        [HttpGet("my-team")]
        public async Task<ActionResult<IEnumerable<TeamMemberDTO>>> GetMyRecruiters()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("preferred_username");

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return Unauthorized("User email claim not found in token.");
            }

            var result = await _teamService.GetRecruitersForLeadAsync(userEmail);

            return Ok(result);
        }

    }
}
