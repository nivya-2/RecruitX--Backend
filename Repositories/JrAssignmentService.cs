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

        //public async Task<(bool Success, string Message, JrAssignment Assignment)> AssignJobRequisitionAsync(AssignJrDTO assignDto)
        public async Task<JrAssignment> AssignJobRequisitionAsync(AssignJrDTO assignDto)
        {
            var jobRequisition = await _context.JobRequisitions.FindAsync(assignDto.JobRequisitionId)
                ?? throw new ArgumentException("Job Requisition not found");

            var assignedToUser = await _context.Users.FindAsync(assignDto.AssignedToUserId)
                ?? throw new ArgumentException("Assigned user not found");

            var assignedByUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == assignDto.AssignedByUsername)
                ?? throw new ArgumentException("Assigning user not found");

            // Check if already assigned
            var existingAssignment = await _context.JrAssignments
                .FirstOrDefaultAsync(a => a.JobRequisitionId == assignDto.JobRequisitionId);

            if (existingAssignment != null && !assignDto.ForceReassign)
            {
                throw new InvalidOperationException("Job Requisition is already assigned. Use force reassignment to override.");
            }

            // If reassigned, remove previous assignment
            if (existingAssignment != null && assignDto.ForceReassign)
            {
                _context.JrAssignments.Remove(existingAssignment);
            }

            var newAssignment = new JrAssignment
            {
                JobRequisitionId = assignDto.JobRequisitionId,
                AssignedTo = assignDto.AssignedToUserId,
                AssignedBy = assignedByUser.Id,
                AssignedAt = DateTime.UtcNow
            };

            _context.JrAssignments.Add(newAssignment);
            await _context.SaveChangesAsync();

            return newAssignment;
        }
    }
}

