using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitX.Models.DTO;
using RecruitX.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDetailsDTO>>> GetEmployeeDetails()
        {
            var employees = await _userService.GetEmployeeDetailsAsync();
            return Ok(employees);
        }


        [HttpPut("set-inactive/{userId:int}")]
        public async Task<IActionResult> SetInactive(int userId)
        {
            try
            {
                var success = await _userService.SetUserInactiveAsync(userId);
                if (!success)
                    return NotFound(new { message = $"User with ID {userId} not found." });

                return Ok(new { message = $"User {userId} marked as inactive successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("set-active/{userId:int}")]
        public async Task<IActionResult> SetActive(int userId)
        {
            try
            {
                var success = await _userService.SetUserActiveAsync(userId);
                if (!success)
                    return NotFound(new { message = $"User with ID {userId} not found." });

                return Ok(new { message = $"User {userId} marked as active successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("set-recruiter-head/{userId:int}")]
        public async Task<IActionResult> SetRecruiterHead(int userId)
        {
            try
            {
                var success = await _userService.SetRecruiterHeadAsync(userId);
                if (!success)
                    return NotFound(new { message = $"User with ID {userId} not found or already a Recruiter Head." });

                return Ok(new { message = $"User {userId} has been set as the new Recruiter Head." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
