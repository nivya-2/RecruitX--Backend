using System.Text.RegularExpressions;
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

        //public async Task<List<CandidateDTO>> GetCandidatesByJobDescriptionIdAsync(int jrId)
        //{
        //    var latestInterviews = await _context.Interviews
        //        .Include(i => i.Application)
        //            .ThenInclude(a => a.Candidate)
        //        .Include(i => i.Application)
        //            .ThenInclude(a => a.JobDescription)
        //        .Where(i => i.Application.JobDescription.JobRequisitionId == jrId)
        //        .OrderByDescending(i => i.CreatedAt)
        //        .ToListAsync();

        //    var grouped = latestInterviews
        //        .GroupBy(i => i.Application.Candidate.Id)
        //        .Select(g => g.First())
        //        .Select(i => new CandidateDTO
        //        {
        //            Id = i.Application.Candidate.Id,
        //            Name = i.Application.Candidate.CandidateName,
        //            MobNumber = i.Application.Candidate.ContactNumber,
        //            Email = i.Application.Candidate.Email,
        //            CurrentEmployer = i.Application.Candidate.CurrentEmployer ?? string.Empty,
        //            TotalExp = $"{i.Application.Candidate.TotalExperienceYears} years",
        //            RelevantExp = $"{i.Application.Candidate.RelevantExperienceYears} years",
        //            Stage = $"{(i.IsTechnicalRound ? "Technical" : "Management")} {i.InterviewCount}"
        //        })
        //        .ToList();

        //    return grouped;
        //}

        //public async Task<List<CandidateDTO>> GetCandidatesByJobDescriptionIdAsync(int jrId)
        //{
        //    var applications = await _context.Applications
        //        .Include(a => a.Candidate)
        //        .Include(a => a.Interviews)
        //        .Include(a => a.JobDescription)
        //        .Where(a => a.JobDescription.JobRequisitionId == jrId)
        //        .ToListAsync();

        //    var candidateDtos = applications.Select(app =>
        //    {
        //        var interviews = app.Interviews.OrderByDescending(i => i.CreatedAt).ToList();

        //        string nextStage;

        //        if (!interviews.Any())
        //        {
        //            // No interview yet
        //            nextStage = "Technical Interview 1";
        //        }
        //        else
        //        {
        //            // Count technical and management interviews separately
        //            int techCount = interviews.Count(i => i.IsTechnicalRound);
        //            int mgmtCount = interviews.Count(i => !i.IsTechnicalRound);

        //            // Assuming next is same type as last, or alternate logic if needed
        //            var last = interviews.First(); // latest
        //            if (last.IsTechnicalRound)
        //                nextStage = $"Technical Interview {techCount + 1}";
        //            else
        //                nextStage = $"Management Round {mgmtCount + 1}";
        //        }

        //        return new CandidateDTO
        //        {
        //            Id = app.Candidate.Id,
        //            Name = app.Candidate.CandidateName,
        //            MobNumber = app.Candidate.ContactNumber,
        //            Email = app.Candidate.Email,
        //            CurrentEmployer = app.Candidate.CurrentEmployer ?? string.Empty,
        //            TotalExp = $"{app.Candidate.TotalExperienceYears} years",
        //            RelevantExp = $"{app.Candidate.RelevantExperienceYears} years",
        //            Stage = nextStage
        //        };
        //    }).ToList();

        //    return candidateDtos;
        //}
        //public async Task<List<CandidateDTO>> GetCandidatesByJobDescriptionIdAsync(int jrId)
        //{
        //    var applications = await _context.Applications
        //        .Include(a => a.Candidate)
        //        .Include(a => a.JobDescription)
        //        .Where(a => a.JobDescription.JobRequisitionId == jrId)
        //        .ToListAsync();

        //    var appIds = applications.Select(a => a.Id).ToList();

        //    var interviews = await _context.Interviews
        //        .Where(i => appIds.Contains(i.ApplicationId))
        //        .ToListAsync();

        //    var candidates = applications.Select(app =>
        //    {
        //        var candidateInterviews = interviews
        //            .Where(i => i.ApplicationId == app.Id)
        //            .ToList();

        //        var techCount = candidateInterviews
        //            .Where(i => i.IsTechnicalRound)
        //            .Max(i => (int?)i.InterviewCount) ?? 0;

        //        var mgmtCount = candidateInterviews
        //            .Where(i => !i.IsTechnicalRound)
        //            .Max(i => (int?)i.InterviewCount) ?? 0;

        //        string currentStage = FormatStageFromStatus(app.Status);
        //        string nextTech = $"Technical Interview {techCount + 1}";
        //        string nextMgmt = $"Management Round {mgmtCount + 1}";

        //        return new CandidateDTO
        //        {
        //            Id = app.Candidate.Id,
        //            Name = app.Candidate.CandidateName,
        //            MobNumber = app.Candidate.ContactNumber,
        //            Email = app.Candidate.Email,
        //            CurrentEmployer = app.Candidate.CurrentEmployer ?? string.Empty,
        //            TotalExp = $"{app.Candidate.TotalExperienceYears} years",
        //            RelevantExp = $"{app.Candidate.RelevantExperienceYears} years",
        //            Stage = $"Current: {currentStage} | Next: {nextTech} / {nextMgmt}"
        //        };
        //    }).ToList();

        //    return candidates;
        //}

        //private string FormatStageFromStatus(ApplicationStatus status)
        //{
        //    // Converts enum like "TechnicalInterview" to "Technical Interview"
        //    return Regex.Replace(status.ToString(), "([a-z])([A-Z])", "$1 $2");
        //}
        public async Task<List<CandidateDTO>> GetCandidatesByJobDescriptionIdAsync(int jrId)
        {
            var applications = await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.JobDescription)
                .Where(a => a.JobDescription.JobRequisitionId == jrId)
                .ToListAsync();

            var appIds = applications.Select(a => a.Id).ToList();

            var interviews = await _context.Interviews
                .Where(i => appIds.Contains(i.ApplicationId))
                .ToListAsync();

            var candidates = applications.Select(app =>
            {
                var candidateInterviews = interviews
                    .Where(i => i.ApplicationId == app.Id)
                    .ToList();

                int techCount = candidateInterviews
                    .Where(i => i.IsTechnicalRound)
                    .Max(i => (int?)i.InterviewCount) ?? 0;

                int mgmtCount = candidateInterviews
                    .Where(i => !i.IsTechnicalRound)
                    .Max(i => (int?)i.InterviewCount) ?? 0;

                string stage;

                if (app.Status == ApplicationStatus.TechnicalInterview)
                {
                    stage = $"Technical Interview {techCount + 1}";
                }
                else if (app.Status == ApplicationStatus.ManagementInterview)
                {
                    stage = $"Management Round {mgmtCount + 1}";
                }
                else
                {
                    stage = FormatStageFromStatus(app.Status); // or "Not Started", if you prefer
                }

                return new CandidateDTO
                {
                    Id = app.Candidate.Id,
                    Name = app.Candidate.CandidateName,
                    MobNumber = app.Candidate.ContactNumber,
                    Email = app.Candidate.Email,
                    CurrentEmployer = app.Candidate.CurrentEmployer ?? string.Empty,
                    TotalExp = $"{app.Candidate.TotalExperienceYears} years",
                    RelevantExp = $"{app.Candidate.RelevantExperienceYears} years",
                    Stage = stage
                };
            }).ToList();

            return candidates;
        }

        private string FormatStageFromStatus(ApplicationStatus status)
        {
            return Regex.Replace(status.ToString(), "([a-z])([A-Z])", "$1 $2");
        }




    }
}
