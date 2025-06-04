// File: RecruitX.Controllers/JobRequisitionController.cs

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;
using RecruitX.Repositories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecruitX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobRequisitionController : ControllerBase
    {
        private readonly IUploadJobRequisitionService _jobRequisitionService;
        //private readonly IJrAssignmentService _assignmentService;
        private readonly ILogger<JobRequisitionController> _logger;
        private readonly AppDbContext _context;

        public JobRequisitionController(
            IUploadJobRequisitionService jobRequisitionService,
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
                _logger.LogWarning("CreateJobRequisition: Model state is invalid. {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            //var username = User.Identity?.Name;

            //if (string.IsNullOrEmpty(username))
            //{
            //    _logger.LogWarning("CreateJobRequisition: Username not found in token.");
            //    return Unauthorized("Username not found in token.");
            //}




            try
            {
                //_logger.LogInformation("Attempting to create job requisition by Username: {Username}. DTO: {@JobRequisitionDto}", username, jobRequisitionDto);

                //JobRequisition createdJobRequisition = await _jobRequisitionService.CreateJobRequisitionAsync(jobRequisitionDto, username);
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
                //_logger.LogError(dbEx, "Database update error occurred while creating Job Requisition. Uploader: {username}. DTO: {@JobRequisitionDto}", username, jobRequisitionDto);

                if (dbEx.InnerException != null)
                {
                    _logger.LogError(dbEx.InnerException, "Inner exception details for DbUpdateException.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while saving to the database: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Generic error occurred while creating Job Requisition. Uploader: {username}. DTO: {@JobRequisitionDto}", username, jobRequisitionDto);
            //    return StatusCode(StatusCodes.Status500InternalServerError, $"An internal server error occurred: {ex.Message}");
            //}
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobRequisition(int id)
        {
            try
            {
                var jobRequisition = await _context.JobRequisitions
                    .Include(jr => jr.JobSkills)
                    .FirstOrDefaultAsync(jr => jr.Id == id);

                if (jobRequisition == null)
                {
                    _logger.LogWarning("DeleteJobRequisition: Job Requisition with ID {Id} not found.", id);
                    return NotFound(new { message = $"Job Requisition with ID {id} not found." });
                }

                // If there are related JobSkills, remove them first due to FK constraints
                if (jobRequisition.JobSkills != null && jobRequisition.JobSkills.Count > 0)
                {
                    _context.JobSkills.RemoveRange(jobRequisition.JobSkills);
                }

                _context.JobRequisitions.Remove(jobRequisition);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted Job Requisition with ID {Id}.", id);
                return Ok(new { message = $"Job Requisition with ID {id} deleted successfully." });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error while deleting Job Requisition with ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error occurred: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting Job Requisition with ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        //[HttpPost("{id}/assign")]
        //public async Task<IActionResult> AssignJobRequisition(int id, [FromBody] AssignJrDTO dto)
        //{
        //    var username = User.FindFirst(ClaimTypes.Name)?.Value;
        //    if (string.IsNullOrEmpty(username))
        //    {
        //        return Unauthorized("Username claim not found. Ensure JWT contains a valid 'Name' claim.");
        //    }

        //    if (id != dto.JobRequisitionId)
        //    {
        //        return BadRequest("Job requisition ID in route and body do not match.");
        //    }

        //    try
        //    {
        //        var assignment = await _assignmentService.AssignJrAsync(dto, username);
        //        return Ok(new { message = "Job requisition assigned successfully.", assignment });
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        _logger.LogWarning(ex, "Unauthorized role.");
        //        return Forbid(ex.Message);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        _logger.LogWarning(ex, "Validation error.");
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        _logger.LogWarning(ex, "Business rule violation.");
        //        return Conflict(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error during assignment.");
        //        return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
        //    }
        //}

    }
}
