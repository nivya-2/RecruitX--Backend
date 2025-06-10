using System.Linq;
using Microsoft.EntityFrameworkCore;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;

namespace RecruitX.Repositories
{
    public class AssignedJrService : IAssignedJrService
    {
        private readonly AppDbContext _context;

        public AssignedJrService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<AssignedJrDTO>> GetAssignedJobRequisitionsByUserNameAsync(int userId)
        {
            // 1. Find User by username, include Employee navigation
            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Employee == null)
            {
                return null;
            }

            int employeeId = user.Id;

            // 2. Query Job Requisitions assigned to this employee (HiringManager == employeeId)
            var jobRequisitions = await _context.JrAssignments
                .Where(jr => jr.AssignedTo == employeeId)
                .Include(jr => jr.JobRequisition).ThenInclude(j => j.Department)
                .Include(jr => jr.JobRequisition).ThenInclude(j => j.Location)
                .Include(jr => jr.JobRequisition).ThenInclude(j => j.HiringManagerEmployee)
                .ToListAsync();

            //// Step 1: Get relevant JobRequisition IDs
            //var jobRequisitionIds = jobRequisitions.Select(jr => jr.JobRequisition.Id).ToList();

            //// Step 2: Get Filled Positions from JobDescriptions
            //var filledPositionMap = await _context.JobDescriptions
            //    .Where(jd => jobRequisitionIds.Contains(jd.JobRequisitionId))
            //    .GroupBy(jd => jd.JobRequisitionId)
            //    .ToDictionaryAsync(
            //        g => g.Key,
            //        g => g.Sum(jd => jd.FilledPositions ?? 0)
            //    );

            // 3. Map to AssignedJrDTO
            var assignedJrs = jobRequisitions.Select(ar => 
            {
                var jr = ar.JobRequisition;
                return new AssignedJrDTO
                {
                    JobId = jr.Id,
                    JobTitle = jr.Role ?? string.Empty,
                    DU = jr.Department?.Name ?? string.Empty,
                    Location = jr.Location?.LocationName ?? string.Empty,
                    Status = jr.JrStatus.ToString(),
                    JrProgress = new
                    {
                        current = _context.JobDescriptions
                            .Where(desc => desc.JobRequisitionId == jr.Id)
                            .Select(desc => desc.FilledPositions)
                            .FirstOrDefault(),// TODO: Replace with actual FilledPositions
                        total = jr.NumPositions
                    },
                    HiringManager = $"{jr.HiringManagerEmployee?.FirstName} {jr.HiringManagerEmployee?.LastName}",
                    AssignedOn = jr.RequestedDate?.ToString("dd-MM-yyyy") ?? string.Empty,
                    CloseBy = jr.IdealStartDate?.ToString("dd-MM-yyyy") ?? string.Empty
                };
            }).ToList();

            return assignedJrs;
        }
    }
}
