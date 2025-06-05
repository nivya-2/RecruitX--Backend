using Microsoft.EntityFrameworkCore;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;

namespace RecruitX.Repositories
{
    public class JrAssignmentService : IJrAssignmentService
    {
        private readonly AppDbContext _context;

        public JrAssignmentService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<JrAssignment> AssignJrAsync(AssignJrDTO dto)
        {
            //var assigner = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == assignedByUsername);
            //if (assigner == null || (assigner.Role.RoleName != "Recruiter Head" && assigner.Role.RoleName != "Recruiter Lead"))
            //{
            //    throw new UnauthorizedAccessException("Only users with roles 'Recruiter Head' or 'Recruiter Lead' can assign job requisitions.");
            //}

            var assignee = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.AssignedTo);
            if (assignee == null)
                throw new ArgumentException($"Assigned user '{dto.AssignedTo}' not found.");

            var jr = await _context.JobRequisitions.FirstOrDefaultAsync(j => j.Id == dto.JobRequisitionId);
            if (jr == null)
                throw new ArgumentException($"Job requisition with ID {dto.JobRequisitionId} not found.");

            //var existingAssignment = await _context.JrAssignments
            //    .Include(a => a.AssignedByUser).ThenInclude(u => u.Role)
            //    .FirstOrDefaultAsync(a => a.JobRequisitionId == dto.JobRequisitionId);

            //if (existingAssignment != null)
            //{
            //    if (existingAssignment.AssignedByUser.Role.RoleName != assigner.Role.RoleName)
            //    {
            //        throw new InvalidOperationException($"Job requisition was assigned by a {existingAssignment.AssignedByUser.Role.RoleName}. You ({assigner.Role.RoleName}) cannot reassign it.");
            //    }
            //    else
            //    {
            //        throw new InvalidOperationException($"Job requisition with ID {dto.JobRequisitionId} is already assigned.");
            //    }
            //}

            var assignment = new JrAssignment
            {
                JobRequisitionId = dto.JobRequisitionId,
                AssignedTo = assignee.Id,
                //AssignedBy = assigner.Id,
                AssignedAt = DateTime.UtcNow
            };

            _context.JrAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return assignment;
        }
    }
}

