using Microsoft.EntityFrameworkCore;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Repositories
{
    public class TeamService : ITeamService
    {
        private readonly AppDbContext _context;

        public TeamService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<TeamMemberDTO>> GetTeamMembersAsync()
        {
            // Step 1: Get all lead-recruiter mappings
            var leadRecruiterMap = await _context.LeadToRecruiters
                .Include(lr => lr.Lead).ThenInclude(u => u.Employee)
                .Include(lr => lr.Recruiter).ThenInclude(u => u.Employee)
                .ToListAsync();

            // Step 2: Group recruiters by lead
            var leadGroups = leadRecruiterMap
                .GroupBy(lr => lr.Lead)
                .ToList();

            // Step 3: Fetch JR assignment counts for all users
            var allUserIds = leadGroups.Select(g => g.Key.Id)
                .Union(leadGroups.SelectMany(g => g.Select(r => r.Recruiter.Id)))
                .Distinct()
                .ToList();

            var jrCounts = await _context.JrAssignments
                .Where(jr => allUserIds.Contains(jr.AssignedTo))
                .GroupBy(jr => jr.AssignedTo)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            var result = new List<TeamMemberDTO>();

            foreach (var group in leadGroups)
            {
                var lead = group.Key;
                var leadEmp = lead.Employee!;
                result.Add(new TeamMemberDTO
                {
                    UserId = leadEmp.Id,
                    MemberName = $"{leadEmp.FirstName} {leadEmp.LastName}",
                    JobTitle = leadEmp.Position,
                    JrAssigned = jrCounts.GetValueOrDefault(lead.Id, 0),
                    Actions = new List<string> { "View assigned JR", "Change Lead" }
                });

                foreach (var recruiter in group.Select(g => g.Recruiter))
                {
                    var recEmp = recruiter.Employee!;
                    result.Add(new TeamMemberDTO
                    {
                        UserId = recEmp.Id,
                        MemberName = $"{recEmp.FirstName} {recEmp.LastName}",
                        JobTitle = recEmp.Position,
                        JrAssigned = jrCounts.GetValueOrDefault(recruiter.Id, 0),
                        ReportingLead = $"{leadEmp.FirstName} {leadEmp.LastName}",
                        Actions = new List<string> { "View assigned JR", "Remove" }
                    });
                }
            }

            return result;
        }
    }
}
