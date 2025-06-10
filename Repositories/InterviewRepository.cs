using Microsoft.EntityFrameworkCore;
using RecruitX.Data;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Repositories
{
    public class InterviewRepository : IInterviewService
    {
        private readonly AppDbContext _context;

        public InterviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InterviewDTO>> GetAllInterviewsAsync()
        {
            var now = DateTime.UtcNow;
            var allInterviews = await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(a => a.Candidate)
                .Include(i => i.Application)
                    .ThenInclude(a => a.JobDescription)
                        .ThenInclude(jd => jd.JobRequisition)
                .Include(i => i.InterviewPanels)
                    .ThenInclude(ip => ip.Employee)
                        .ThenInclude(e => e.Department)
                .OrderBy(i => i.ScheduledAt)
                .ToListAsync();

            var interviewDTOs = allInterviews.Select(interview =>
            {
                // No round count is needed. We'll simply set:
                var typeLabel = interview.IsTechnicalRound ? "Technical" : "Management";
                var isUpcoming = interview.ScheduledTo > now;

                return new InterviewDTO
                {
                    InterviewId = interview.Id,
                    CandidateName = interview.Application.Candidate.CandidateName,
                    JobRole = _context.JobRequisitions
                                .Where(jr => jr.Id == interview.Application.JobDescription.JobRequisitionId)
                                .Select(jr => jr.Role)
                                .FirstOrDefault() ?? string.Empty,
                    Date = interview.ScheduledAt.Date,
                    Time = $"{interview.ScheduledAt:hh:mm tt} - {interview.ScheduledTo:hh:mm tt}",
                    InterviewRound = typeLabel, // simply "Technical" or "Management"
                    InterviewerName = interview.InterviewPanels
                                .Select(p => p.Employee.FirstName + " " + p.Employee.LastName)
                                .FirstOrDefault() ?? "N/A",
                    JobDescription = interview.Application.JobDescription.JobRequisitionId,
                    CreatedDate = interview.CreatedAt,
                    Status = isUpcoming ? "Upcoming" : "Completed"
                };
            }).ToList();

            return interviewDTOs;
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
                app.Status == ApplicationStatus.ManagementInterview ||
                app.Status == ApplicationStatus.TechnicalInterview)
            &&
            !jd.Applications.Any(app => app.Interviews.Any(i =>
            i.Status==InterviewStatus.Scheduled ||
            i.Status==InterviewStatus.PendingShortlist)) //not pendingshortlist or scheduled
        )
        .Select(jd => new ToScheduleDto
        {
            Id = _context.JobRequisitions
        .Where(jr => jr.Id == jd.JobRequisitionId)
        .Select(jr => jr.Id)
        .FirstOrDefault(),

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

            CreatedDate = jd.CreatedAt,
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
             .Include(i => i.Application)
                    .ThenInclude(app => app.JobDescription)
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
                    Id = "CAN_" + interview.Application.Candidate.Id.ToString("D3"),
                    InterviewId = interview.Id,
                    JdId = interview.Application.JobDescription.JobRequisitionId,
                    Name = interview.Application.Candidate.CandidateName,
                    InterviewDate = interview.ScheduledAt,
                    InterviewType = roundLabel,
                    Actions = new List<string> { "Shortlist" }
                });
            }

            return result;
        }

        public async Task<List<CandidateDTO>> GetCandidatesByJobDescriptionIdAsync(int jdId)
        {
            var latestInterviews = await _context.Interviews
                .Where(i => i.Application.JobDescriptionId == jdId)
                .Include(i => i.Application)
                    .ThenInclude(a => a.Candidate)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var grouped = latestInterviews
                .GroupBy(i => i.Application.Candidate.Id)
                .Select(g => g.First())
                .Select(i => new CandidateDTO
                {
                    Id = i.Application.Candidate.Id,
                    CandidateName = i.Application.Candidate.CandidateName,
                    MobileNumber = i.Application.Candidate.ContactNumber,
                    Email = i.Application.Candidate.Email,
                    CurrentEmployer = i.Application.Candidate.CurrentEmployer ?? string.Empty,
                    TotalExperience = $"{i.Application.Candidate.TotalExperienceYears} years",
                    RelevantExperience = $"{i.Application.Candidate.RelevantExperienceYears} years",
                    Stage = $"{(i.IsTechnicalRound ? "Technical" : "Management")} {i.InterviewCount}"
                })
                .ToList();

            return grouped;
        }


    }
}
