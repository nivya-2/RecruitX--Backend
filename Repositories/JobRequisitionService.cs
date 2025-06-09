// RecruitX.Repositories/JobRequisitionService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitX.Data;
using RecruitX.DTOs;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitX.Repositories
{
    public class JobRequisitionService : IJobRequisitionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JobRequisitionService> _logger;

        public JobRequisitionService(AppDbContext context, ILogger<JobRequisitionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<int?> ResolveEmployeeIdByNameAsync(string? name, string employeeRoleForLog)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var normalizedName = name.Trim().ToLower();
            Employee? employee = null;

            if (normalizedName.Contains('@'))
            {
                employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email.ToLower() == normalizedName);
            }

            if (employee == null)
            {
                employee = await _context.Employees
                    .FirstOrDefaultAsync(e => (e.FirstName.ToLower() + " " + e.LastName.ToLower()) == normalizedName);
            }

            if (employee == null)
            {
                _logger.LogWarning($"{employeeRoleForLog} with name/email '{name}' not found.");
                throw new InvalidOperationException($"{employeeRoleForLog} with name/email '{name}' not found.");
            }

            _logger.LogInformation($"Resolved {employeeRoleForLog} '{name}' to Employee ID: {employee.Id}.");
            return employee.Id;
        }

        //public async Task<JobRequisition> CreateJobRequisitionAsync(UploadJrDTO dto, string username)
        public async Task<JobRequisition> CreateJobRequisitionAsync(UploadJrDTO dto)

        {
            //_logger.LogInformation($"Job Requisition upload attempt by Employee: {username}. DTO: {@dto}");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int? requestedById = await ResolveEmployeeIdByNameAsync(dto.RequestedByName, "RequestedBy Employee");
                int? hiringManagerId = await ResolveEmployeeIdByNameAsync(dto.HiringManagerName, "HiringManager Employee");
                int? designatedCreatorId = await ResolveEmployeeIdByNameAsync(dto.CreatedByEmployeeName, "Designated Creator Employee");

                int? finalClientId = null;
                if (!string.IsNullOrWhiteSpace(dto.ClientName))
                {
                    var existingClient = await _context.Clients
                        .FirstOrDefaultAsync(c => c.ClientName.ToLower() == dto.ClientName.ToLower() &&
                                                  (string.IsNullOrEmpty(dto.ClientCountry) || c.ClientCountry.ToLower() == dto.ClientCountry.ToLower()));

                    if (existingClient != null)
                        finalClientId = existingClient.Id;
                    else
                    {
                        var newClient = new Client { ClientName = dto.ClientName, ClientCountry = dto.ClientCountry };
                        _context.Clients.Add(newClient);
                        await _context.SaveChangesAsync();
                        finalClientId = newClient.Id;
                    }
                }

                int? finalLocationId = null;
                if (!string.IsNullOrWhiteSpace(dto.LocationName))
                {
                    var existingLocation = await _context.Locations
                        .FirstOrDefaultAsync(l => l.LocationName.ToLower() == dto.LocationName.ToLower() &&
                                                  (string.IsNullOrEmpty(dto.LocationCountry) || l.Country.ToLower() == dto.LocationCountry.ToLower()));

                    if (existingLocation != null)
                        finalLocationId = existingLocation.Id;
                    else
                    {
                        var newLocation = new Location { LocationName = dto.LocationName, Country = dto.LocationCountry };
                        _context.Locations.Add(newLocation);
                        await _context.SaveChangesAsync();
                        finalLocationId = newLocation.Id;
                    }
                }

                var departmentEntity = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Name.ToLower() == dto.DepartmentName.ToLower());
                if (departmentEntity == null)
                {
                    departmentEntity = new Department { Name = dto.DepartmentName };
                    _context.Departments.Add(departmentEntity);
                    await _context.SaveChangesAsync();
                }
                int finalDepartmentId = departmentEntity.Id;

                var jobRequisition = new JobRequisition
                {
                    Role = dto.Role,
                    RequestedDate = dto.RequestedDate,
                    RequestedBy = requestedById,
                    DepartmentId = finalDepartmentId,
                    HiringManager = hiringManagerId,
                    CreatedBy = designatedCreatorId,
                    Qualification = dto.Qualification,
                    TotalExperienceYears = (int?)dto.TotalExperienceYears,
                    RelevantExperienceYears = (int?)dto.RelevantExperienceYears,
                    LocationId = finalLocationId,
                    ExpectedOnboardingDate = dto.ExpectedOnboardingDate,
                    NumPositions = dto.NumPositions,
                    WorkShift = dto.WorkShift,
                    HasOnsiteOpportunity = dto.HasOnsiteOpportunity,
                    IsBillable = dto.IsBillable,
                    HasClientInterview = dto.HasClientInterview,
                    ProjectName = dto.ProjectName,
                    ProjectRole = dto.ProjectRole,
                    ClientId = finalClientId,
                    ExpectedSalaryMinimum = dto.ExpectedSalaryMinimum,
                    ExpectedSalaryMaximum = dto.ExpectedSalaryMaximum,
                    JobPurpose = dto.JobPurpose,
                    JobDuties = dto.JobDuties,
                    JobSpecification = dto.JobSpecification,
                    IdealStartDate = dto.IdealStartDate,
                    CreatedAt = DateTime.UtcNow,
                    IsClosed = false,
                    JrStatus = JobStatus.Open
                };
                _context.JobRequisitions.Add(jobRequisition);
                await _context.SaveChangesAsync();

                if (dto.Skills != null && dto.Skills.Any())
                {
                    foreach (var skillInput in dto.Skills)
                    {
                        var skillEntity = await _context.Skills
                            .FirstOrDefaultAsync(s => s.SkillName.ToLower() == skillInput.SkillName.ToLower());

                        if (skillEntity == null)
                        {
                            skillEntity = new Skill { SkillName = skillInput.SkillName };
                            _context.Skills.Add(skillEntity);
                            await _context.SaveChangesAsync();
                        }

                        _context.JobSkills.Add(new JobSkill
                        {
                            JobRequisitionId = jobRequisition.Id,
                            SkillId = skillEntity.Id,
                            SkillType = skillInput.SkillType
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                if (jobRequisition.HasOnsiteOpportunity && dto.OnSiteDetails != null)
                {
                    var onsiteDetail = new OnSiteDetail
                    {
                        JrId = jobRequisition.Id,
                        ContractType = dto.OnSiteDetails.ContractType,
                        Rate = dto.OnSiteDetails.Rate,
                        WorkLocation = dto.OnSiteDetails.WorkLocation,
                        PreferredVisaStatus = dto.OnSiteDetails.PreferredVisaStatus,
                        ContractDuration = dto.OnSiteDetails.ContractDuration,
                        PreferredTimeZone = dto.OnSiteDetails.PreferredTimeZone,
                        ClientBackground = dto.OnSiteDetails.ClientBackground,
                        ClientLocation = dto.OnSiteDetails.ClientLocation,
                        ReportingTo = dto.OnSiteDetails.ReportingTo,
                        InterviewProcess = dto.OnSiteDetails.InterviewProcess,
                        IdealStartDate = dto.OnSiteDetails.IdealStartDate,
                        IsH1TransferAccepted = dto.OnSiteDetails.IsH1TransferAccepted,
                        IsTravelRequired = dto.OnSiteDetails.IsTravelRequired,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.OnSiteDetails.Add(onsiteDetail);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                _logger.LogInformation($"Transaction committed. JobRequisition created successfully with ID: {jobRequisition.Id}.");

                return await _context.JobRequisitions
                    .Include(jr => jr.JobSkills).ThenInclude(js => js.Skill)
                    .Include(jr => jr.Client)
                    .Include(jr => jr.Location)
                    .Include(jr => jr.Department)
                    .Include(jr => jr.RequestedByEmployee)
                    .Include(jr => jr.HiringManagerEmployee)
                    .Include(jr => jr.CreatedByEmployee)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(jr => jr.Id == jobRequisition.Id);
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                //_logger.LogWarning(ex, $"Validation error during job requisition creation by uploader  {username}. Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                //_logger.LogError(ex, $"Unexpected error during job requisition creation by uploader  {username}.");
                throw;
            }
        }
        public async Task<IEnumerable<JobRequisitionDto>> GetAllAsync()
        {
            var assignments = await _context.JrAssignments
                .Select(a => a.JobRequisitionId)
                .Distinct()
                .ToListAsync();

            return await _context.JobRequisitions
                .AsNoTracking()
                .Include(jr => jr.Department)
                .Include(jr => jr.Location)
                .Include(jr => jr.HiringManagerEmployee)
                .Select(jr => new JobRequisitionDto
                {
                    Id = jr.Id,
                    Role = jr.Role,
                    DepartmentName = jr.Department.Name,
                    LocationName = jr.Location != null ? jr.Location.LocationName : null,
                    HiringManagerName = jr.HiringManagerEmployee.FirstName + " " + jr.HiringManagerEmployee.LastName,
                    RequestedOn = jr.RequestedDate,
                    IsAssigned = assignments.Contains(jr.Id)
                })
                .ToListAsync();
        }
        public async Task<bool> DeleteJobRequisitionAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var jobRequisition = await _context.JobRequisitions
                    .Include(jr => jr.JobSkills)
                    .IgnoreQueryFilters() // ✅ required to find even soft-deleted
                    .FirstOrDefaultAsync(jr => jr.Id == id);

                if (jobRequisition == null)
                {
                    _logger.LogWarning("Attempted to delete a non-existent Job Requisition with ID {Id}", id);
                    await transaction.RollbackAsync();
                    return false;
                }

                // ✅ Soft-delete only
                jobRequisition.DeletedAt = DateTime.UtcNow;

                // Optionally soft-delete JobSkills or keep them

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Soft-deleted Job Requisition with ID {Id}.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting Job Requisition with ID {Id}", id);
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<JobRequisitionSummaryDto>> GetOpenJobSummariesAsync()
        {
            return await _context.JobRequisitions
                .Where(jr =>
                    !jr.IsClosed &&
                    jr.DeletedAt == null &&
                    !_context.JrAssignments.Any(ja => ja.JobRequisitionId == jr.Id)
                )
                .Include(jr => jr.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(jr => jr.Location)
                .Select(jr => new JobRequisitionSummaryDto
                {
                    Id = jr.Id,
                    Title = jr.Role,
                    Skills = jr.JobSkills.Select(js => js.Skill.SkillName).ToList(),
                    OpenPositions = jr.NumPositions,
                    PostedDate = jr.RequestedDate,
                    Location = jr.Location != null ? jr.Location.LocationName : null
                })
                .AsNoTracking()
                .ToListAsync();
        }
        // Your existing service method, but now returning Task<JrAssignmentDto>

        public async Task<JrAssignmentDto> AssignJrAsync(AssignJrDTO dto, User user)
        {
            // --- All of your existing validation and setup logic is PERFECT. Keep it. ---
            var assignee = await _context.Users
                // If you need the FullName for the DTO, you might need to include related data.
                // .Include(u => u.Employee) // Example if FullName is on Employee
                .FirstOrDefaultAsync(u => u.Id == dto.AssignedTo);
            if (assignee == null)
                throw new ArgumentException($"Assigned user '{dto.AssignedTo}' not found.");

            var jr = await _context.JobRequisitions.FirstOrDefaultAsync(j => j.Id == dto.JobRequisitionId);
            if (jr == null)
                throw new ArgumentException($"Job requisition with ID {dto.JobRequisitionId} not found.");

            var existingAssignment = await _context.JrAssignments
                .FirstOrDefaultAsync(a => a.JobRequisitionId == dto.JobRequisitionId);

            if (existingAssignment != null)
                throw new InvalidOperationException($"Job requisition with ID {dto.JobRequisitionId} is already assigned.");

            // --- Your entity creation logic is also PERFECT. Keep it. ---
            var assignmentEntity = new JrAssignment
            {
                JobRequisitionId = dto.JobRequisitionId,
                AssignedTo = assignee.Id,
                AssignedBy = user.Id, // Use the passed user object here
                AssignedAt = DateTime.UtcNow
            };

            _context.JrAssignments.Add(assignmentEntity);
            await _context.SaveChangesAsync();

            // --- NEW PART: Map the saved entity to a DTO before returning ---
            // After SaveChanges, assignmentEntity now has its generated ID.

            var resultDto = new JrAssignmentDto
            {
                Id = assignmentEntity.Id, // The new ID from the database
                JobRequisitionId = assignmentEntity.JobRequisitionId,
                AssignedAt = assignmentEntity.AssignedAt,
                AssignedBy = new UserSummaryDto
                {
                    Id = user.Id,
                    FullName = user.Username // Assuming 'user' object has this data
                },
                AssignedTo = new UserSummaryDto
                {
                    Id = assignee.Id,
                    FullName = assignee.Username // Assuming 'assignee' object has this data
                }
            };

            // Return the DTO, not the database entity
            return resultDto;
        }

        public async Task<IEnumerable<TrackJobRequisitionDTO>> GetAllAssignedJobRequisitionsAsync()
        {
            _logger.LogInformation("Fetching all assigned job requisitions for Head role.");

            var query = _context.JrAssignments
                .AsNoTracking()
                // Use GroupJoin for a reliable LEFT JOIN to JobDescriptions
                .GroupJoin(
                    _context.JobDescriptions,
                    assignment => assignment.JobRequisitionId, // Key from left table (JrAssignment)
                    jd => jd.JobRequisitionId,                 // Key from right table (JobDescription)
                    (assignment, jds) => new { assignment, jds } // Intermediate result
                )
                .SelectMany(
                    temp => temp.jds.DefaultIfEmpty(), // This flattens the group, making it a LEFT JOIN
                    (parent, jd) => new // Project into a final anonymous type
                    {
                        Assignment = parent.assignment,
                        JobDescription = jd // jd will be null if no match was found
                    }
                )
                // Eagerly load all related entities needed for the DTO
                .Include(a => a.Assignment.JobRequisition).ThenInclude(jr => jr.Department)
                .Include(a => a.Assignment.JobRequisition).ThenInclude(jr => jr.Location)
                .Include(a => a.Assignment.JobRequisition).ThenInclude(jr => jr.HiringManagerEmployee)
                .Include(a => a.Assignment.AssignedToUser);

            // Final projection into the DTO
            var result = await query
                .Select(data => new TrackJobRequisitionDTO
                {
                    Id = data.Assignment.JobRequisition.Id,
                    Role = data.Assignment.JobRequisition.Role,
                    DepartmentName = data.Assignment.JobRequisition.Department != null ? data.Assignment.JobRequisition.Department.Name : "N/A",
                    LocationName = data.Assignment.JobRequisition.Location != null ? data.Assignment.JobRequisition.Location.LocationName : "N/A",
                    HiringManagerName = data.Assignment.JobRequisition.HiringManagerEmployee != null ? $"{data.Assignment.JobRequisition.HiringManagerEmployee.FirstName} {data.Assignment.JobRequisition.HiringManagerEmployee.LastName}".Trim() : "N/A",
                    status = data.Assignment.JobRequisition.JrStatus.ToString(),
                    NumPositions = data.Assignment.JobRequisition.NumPositions,
                    FilledPositions = data.JobDescription != null ? data.JobDescription.FilledPositions : 0, // Safely access JobDescription
                    assignedTo = data.Assignment.AssignedToUser != null ? data.Assignment.AssignedToUser.Username : "Unassigned",
                    assignedOn = DateOnly.FromDateTime(data.Assignment.AssignedAt),
                    CloseBy = data.Assignment.JobRequisition.IdealStartDate.HasValue ,
                })
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<TrackJobRequisitionDTO>> GetTrackedJrsForLeadAsync(string leadUserEmail)
        {
            _logger.LogInformation("Fetching tracked job requisitions for lead: {LeadEmail}", leadUserEmail);

            var lead = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.ToLower() == leadUserEmail.ToLower());
            if (lead == null)
            {
                _logger.LogWarning("Lead with email {LeadEmail} not found.", leadUserEmail);
                return Enumerable.Empty<TrackJobRequisitionDTO>();
            }

            var recruiterIds = await _context.LeadToRecruiters
                .Where(ltr => ltr.Id == lead.Id)
                .Select(ltr => ltr.RecruiterId)
                .ToListAsync();

            if (!recruiterIds.Any())
            {
                _logger.LogInformation("Lead ID {LeadId} has no recruiters assigned. Returning empty list.", lead.Id);
                return Enumerable.Empty<TrackJobRequisitionDTO>();
            }

            // The query structure is identical to the one above, with an added .Where clause
            var query = _context.JrAssignments
                .AsNoTracking()
                .Where(a => recruiterIds.Contains(a.AssignedTo)) // <-- Filter by the lead's team
                .GroupJoin(
                    _context.JobDescriptions,
                    assignment => assignment.JobRequisitionId,
                    jd => jd.JobRequisitionId,
                    (assignment, jds) => new { assignment, jds }
                )
                .SelectMany(
                    temp => temp.jds.DefaultIfEmpty(),
                    (parent, jd) => new { parent.assignment, JobDescription = jd }
                )
                .Include(a => a.assignment.JobRequisition).ThenInclude(jr => jr.Department)
                .Include(a => a.assignment.JobRequisition).ThenInclude(jr => jr.Location)
                .Include(a => a.assignment.JobRequisition).ThenInclude(jr => jr.HiringManagerEmployee)
                .Include(a => a.assignment.AssignedToUser);

            var result = await query
                .Select(data => new TrackJobRequisitionDTO
                {
                    Id = data.assignment.JobRequisition.Id,
                    Role = data.assignment.JobRequisition.Role,
                    DepartmentName = data.assignment.JobRequisition.Department != null ? data.assignment.JobRequisition.Department.Name : "N/A",
                    LocationName = data.assignment.JobRequisition.Location != null ? data.assignment.JobRequisition.Location.LocationName : "N/A",
                    HiringManagerName = data.assignment.JobRequisition.HiringManagerEmployee != null ? $"{data.assignment.JobRequisition.HiringManagerEmployee.FirstName} {data.assignment.JobRequisition.HiringManagerEmployee.LastName}".Trim() : "N/A",
                    status = data.assignment.JobRequisition.JrStatus.ToString(),
                    NumPositions = data.assignment.JobRequisition.NumPositions,
                    FilledPositions = data.JobDescription != null ? data.JobDescription.FilledPositions : 0,
                    assignedTo = data.assignment.AssignedToUser != null ? data.assignment.AssignedToUser.Username : "Unassigned",
                    assignedOn = DateOnly.FromDateTime(data.assignment.AssignedAt),
                    CloseBy = data.assignment.JobRequisition.IdealStartDate.HasValue ? (data.assignment.JobRequisition.IdealStartDate.Value) : default
                })
                .ToListAsync();

            return result;
        }

    }
}
