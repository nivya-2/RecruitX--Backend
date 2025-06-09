// File: RecruitX.Controllers/JobRequisitionController.cs

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitX.DTOs;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;
using RecruitX.Repositories;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobRequisitionController : ControllerBase
    {
        private readonly IJobRequisitionService _jobRequisitionService;
        //private readonly IJrAssignmentService _assignmentService;
        private readonly ILogger<JobRequisitionController> _logger;
        private readonly AppDbContext _context;

        public JobRequisitionController(
            IJobRequisitionService jobRequisitionService,
            ILogger<JobRequisitionController> logger,
            AppDbContext context,
            IJrAssignmentService assignmentService)
        {
            _jobRequisitionService = jobRequisitionService;
            _logger = logger;
            _context = context;
            //_assignmentService = assignmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobRequisition([FromBody] UploadJrDTO jobRequisitionDto)
        {
            if (!ModelState.IsValid)
            {
                // Log detailed validation errors for diagnostics
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    var errors = entry.Value.Errors;
                    foreach (var error in errors)
                    {
                        _logger.LogWarning("Validation error on field '{Field}': {ErrorMessage}", key, error.ErrorMessage);
                    }
                }

                // Return detailed errors to client for debugging
                return BadRequest(ModelState);
            }

            try
            {
                JobRequisition createdJobRequisition = await _jobRequisitionService.CreateJobRequisitionAsync(jobRequisitionDto);

                _logger.LogInformation("Successfully created job requisition with ID: {JobRequisitionId}", createdJobRequisition.Id);

                return CreatedAtAction(nameof(GetJobRequisitionById), new { id = createdJobRequisition.Id }, createdJobRequisition);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Failed to create job requisition due to invalid argument: {Message}", argEx.Message);
                return BadRequest(new { message = argEx.Message });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null)
                {
                    _logger.LogError(dbEx.InnerException, "Inner exception details for DbUpdateException.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while saving to the database: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobRequisitionById(int id)
        {
            var jobRequisition = await _context.JobRequisitions
                .Include(jr => jr.Client)
                .Include(jr => jr.Location)
                .Include(jr => jr.Department)
                .Include(jr => jr.JobSkills).ThenInclude(js => js.Skill)
                .Include(jr => jr.RequestedByEmployee)
                .Include(jr => jr.HiringManagerEmployee)
                .Include(jr => jr.CreatedByEmployee)
                .AsNoTracking()
                .FirstOrDefaultAsync(jr => jr.Id == id);

            if (jobRequisition == null)
            {
                _logger.LogWarning("GetJobRequisitionById: Job Requisition with ID {JobRequisitionId} not found.", id);
                return NotFound();
            }

            return Ok(jobRequisition);
        }
        [HttpGet("info/{id}")]
        public async Task<IActionResult> GetJobRequisitionInfoById(int id)
        {
            var job = await _context.JobRequisitions
                .Include(jr => jr.Department)
                .Include(jr => jr.Location)
                .Include(jr => jr.HiringManagerEmployee)
                .Include(jr => jr.CreatedByEmployee)
                    .ThenInclude(emp => emp.User)
                        .ThenInclude(u => u.Role)
                .AsNoTracking()
                .Where(jr => jr.Id == id)
                .Select(jr => new JobRequisitionInfoDto(
                    jr.Id,
                    jr.Role, // JobTitle
                    jr.Department.Name,
                    jr.Location.LocationName,
                    jr.NumPositions ,
                    jr.RequestedDate,
                    jr.HiringManagerEmployee.FirstName + " " + jr.HiringManagerEmployee.LastName,
                    jr.CreatedByEmployee.FirstName + " " + jr.CreatedByEmployee.LastName,
                    jr.CreatedByEmployee.User.Role.RoleName,
                    jr.Qualification
                ))
                .FirstOrDefaultAsync();

            if (job == null)
            {
                _logger.LogWarning("GetJobRequisitionInfoById: ID {Id} not found.", id);
                return NotFound();
            }

            return Ok(job);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobRequisition(int id)
        {
            try
            {
                var success = await _jobRequisitionService.DeleteJobRequisitionAsync(id);

                if (!success)
                {
                    _logger.LogWarning("DeleteJobRequisition: Job Requisition with ID {Id} not found.", id);
                    return NotFound(new { message = $"Job Requisition with ID {id} not found." });
                }

                _logger.LogInformation("Deleted Job Requisition with ID {Id}.", id);
                // Return 204 NoContent, which is standard for a successful DELETE with no content to return.
                // The frontend will receive a success signal and proceed with its `next:` block.
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting Job Requisition with ID {Id}.", id);
                return StatusCode(500, new { message = $"An internal server error occurred: {ex.Message}" });
            }
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobRequisitionDto>>> GetAll()
        {
            var result = await _jobRequisitionService.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("unassigned")]
        public async Task<IActionResult> GetOpenJobSummaries()
        {
            var result = await _jobRequisitionService.GetOpenJobSummariesAsync();
            return Ok(result);
        }



        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignJobRequisition(int id, [FromBody] AssignJrDTO dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                     ?? User.FindFirst("preferred_username")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("No email found.");
            }

            var user = _context.Users
                .Include(u => u.Employee)
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower()) ;

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            try
            {
                var assignment = await _jobRequisitionService.AssignJrAsync(dto,user);
                return Ok(new { message = "Job requisition assigned successfully.", assignment });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized role.");
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error.");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation.");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during assignment.");
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

    }
}
