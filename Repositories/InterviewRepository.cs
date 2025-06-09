using Microsoft.EntityFrameworkCore;
using RecruitX.Data;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Repositories
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly AppDbContext _context;

        public InterviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InterviewDTO>> GetAllInterviewsAsync()
        {
            return await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(a => a.Candidate)
                .Include(i => i.Application)
                    .ThenInclude(a => a.JobDescription)
                        .ThenInclude(jd => jd.JobRequisition)
                .Include(i => i.InterviewPanels)
                    .ThenInclude(ip => ip.Employee)
                        .ThenInclude(e => e.Department)
                .Select(i => new InterviewDTO
                {
                    CandidateName = i.Application.Candidate.CandidateName,
                    //JobRole = i.Application.JobDescription.JobRequisition.Role,
                    JobRole = _context.JobRequisitions
                    .Where(jr => jr.Id == i.Application.JobDescription.JobRequisitionId)
                    .Select(jr => jr.Role)
                    .FirstOrDefault(),
                    Date = i.ScheduledAt.ToString("dd/MM/yyyy hh:mm tt"),
                    Time = i.ScheduledTo.ToString("dd/MM/yyyy hh:mm tt"),
                    InterviewRound = i.Status.ToString(), // replace with real round logic if needed
                    InterviewerName = i.InterviewPanels
                        .Select(p => p.Employee.FirstName + " " + p.Employee.LastName)
                        .FirstOrDefault(),
                    InterviewerDeliveryUnit = i.InterviewPanels
                        .Select(p => p.Employee.Department.Name)
                        .FirstOrDefault(),
                    CreatedDate = i.CreatedAt.ToString("dd/MM/yyyy")
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ToScheduleDto>> GetToScheduleInterviewsAsync()
        {
            return await _context.JobDescriptions
        .Include(jd => jd.JobRequisition)
            .ThenInclude(jr => jr.Department)
        .Include(jd => jd.JobRequisition)
            .ThenInclude(jr => jr.Location)
        .Include(jd => jd.Applications)
            .ThenInclude(app => app.Interviews)
        .Where(jd =>
            jd.Applications.Any(app =>
                app.Status == ApplicationStatus.Applied ||
                app.Status == ApplicationStatus.TechnicalInterview)
            &&
            !jd.Applications.Any(app => app.Interviews.Any())
        )
        .Select(jd => new ToScheduleDto
        {
            Id = jd.Id.ToString(),

            RoleTitle = _context.JobRequisitions
        .Where(jr => jr.Id == jd.JobRequisitionId)
        .Select(jr => jr.Role)
        .FirstOrDefault(),

            DeliveryUnit = _context.JobRequisitions
        .Where(jr => jr.Id == jd.JobRequisitionId)
        .Select(jr => jr.Department.Name)
        .FirstOrDefault(),

            Location = _context.JobRequisitions
        .Where(jr => jr.Id == jd.JobRequisitionId)
        .Select(jr => jr.Location.LocationName)
        .FirstOrDefault(),

            Experience = _context.JobRequisitions
        .Where(jr => jr.Id == jd.JobRequisitionId)
        .Select(jr => jr.RelevantExperienceYears ?? 0)
        .FirstOrDefault(),

            CreatedDate = jd.CreatedAt.ToString("dd-MM-yyyy"),
            AssoJr = jd.JobRequisitionId.ToString(),
            Actions = new List<string> { "Schedule" }
        })
        .ToListAsync();
        }
      
        public async Task<IEnumerable<ToShortlistDto>> GetInterviewsToShortlistAsync()
        {
            var now = DateTime.UtcNow;

            // Step 1: Get all interviews that are completed but pending shortlisting
            var interviews = await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(app => app.Candidate)
                .Where(i =>
                    //i.ScheduledTo < now &&
                    i.Status == InterviewStatus.PendingShortlist // enum-based filtering
                )
                .OrderBy(i => i.ScheduledAt)
                .ToListAsync();

            var result = new List<ToShortlistDto>();

            foreach (var interview in interviews)
            {
                // Count how many technical/management interviews happened before this
                var previousRounds = interviews
                    .Where(x =>
                        x.ApplicationId == interview.ApplicationId &&
                        x.IsTechnicalRound == interview.IsTechnicalRound &&
                        x.ScheduledAt < interview.ScheduledAt)
                    .Count();

                var roundLabel = interview.IsTechnicalRound
                    ? $"Technical L{previousRounds + 1}"
                    : $"Management L{previousRounds + 1}";

                result.Add(new ToShortlistDto
                {
                    Id = "CAN" + interview.Application.Candidate.Id.ToString("D3"),
                    Name = interview.Application.Candidate.CandidateName,
                    InterviewDate = interview.ScheduledAt.ToString("dd/MM/yyyy"),
                    InterviewType = roundLabel,
                    Actions = new List<string> { "Shortlist" }
                });
            }

            return result;
        }
    }
}
