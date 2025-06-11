using Microsoft.EntityFrameworkCore;
using RecruitX.Data;
using RecruitX.Interfaces;
using RecruitX.Models;
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

        public async Task<List<CandidateDTO>> GetCandidatesByJobDescriptionIdAsync(int jrId)
        {
            var latestInterviews = await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(a => a.Candidate)
                .Include(i => i.Application)
                    .ThenInclude(a => a.JobDescription)
                .Where(i => i.Application.JobDescription.JobRequisitionId == jrId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var grouped = latestInterviews
                .GroupBy(i => i.Application.Candidate.Id)
                .Select(g => g.First())
                .Select(i => new CandidateDTO
                {
                    Id = i.Application.Candidate.Id,
                    Name = i.Application.Candidate.CandidateName,
                    MobNumber = i.Application.Candidate.ContactNumber,
                    Email = i.Application.Candidate.Email,
                    CurrentEmployer = i.Application.Candidate.CurrentEmployer ?? string.Empty,
                    TotalExp = $"{i.Application.Candidate.TotalExperienceYears} years",
                    RelevantExp = $"{i.Application.Candidate.RelevantExperienceYears} years",
                    Stage = $"{(i.IsTechnicalRound ? "Technical" : "Management")} {i.InterviewCount}"
                })
                .ToList();

            return grouped;
        }

        private async Task<Application?> FindApplicationAsync(int jobRequisitionId, int candidateId)
        {
            // We must join through JobDescription to link a JR to an Application.
            return await _context.Applications
                .Include(app => app.JobDescription)
                .FirstOrDefaultAsync(app =>
                    app.CandidateId == candidateId &&
                    app.JobDescription.JobRequisitionId == jobRequisitionId
                );
        }

        /// <summary>
        /// Shortlists a candidate, moving them to the next logical step (e.g., TechnicalInterview).
        /// </summary>
        public async Task<bool> ShortlistCandidateAsync(int jobRequisitionId, int candidateId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var application = await FindApplicationAsync(jobRequisitionId, candidateId);
                if (application == null)
                {
                    return false;
                }

                var interview = await _context.Interviews
                   .FirstOrDefaultAsync(i => i.ApplicationId == application.Id);

                if(interview!=null)
                {
                    interview.InterviewCount = 1;

                }
                else
                {
                    Console.WriteLine("interview does not exist");
                }

                    var oldStatus = application.Status;
                ApplicationStatus newStatus;

                // --- THIS IS THE NEW STATE MACHINE LOGIC ---
                switch (oldStatus)
                {

                    case ApplicationStatus.TechnicalInterview:
                        newStatus = ApplicationStatus.ManagementInterview;
                        break;

                    case ApplicationStatus.ManagementInterview:
                        newStatus = ApplicationStatus.DocumentationVerified;
                        break;

                    // If the application is in any other state, we cannot "advance" it with this action.
                    default:
                        return false; // Action is not applicable in the current state.
                }
                // ------------------------------------------

                application.Status = newStatus;
                _context.ApplicationStatusHistories.Add(new ApplicationStatusHistory
                {
                    ApplicationId = application.Id,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    ChangedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        /// <summary>
        /// Rejects a candidate's application.
        /// </summary>
        public async Task<bool> RejectCandidateAsync(int jobRequisitionId, int candidateId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var application = await FindApplicationAsync(jobRequisitionId, candidateId);
                if (application == null)
                {
                    return false;
                }

                // Business rule: Cannot reject an already finished application.
                if (application.Status == ApplicationStatus.Joined || application.Status == ApplicationStatus.Rejected)
                {
                    return false;
                }

                var interview = await _context.Interviews
                 .FirstOrDefaultAsync(i => i.ApplicationId == application.Id);

                if (interview != null)
                {
                    interview.InterviewCount = 1;

                }
                else
                {
                    Console.WriteLine("interview does not exist");
                }

                var oldStatus = application.Status;
                var newStatus = ApplicationStatus.Rejected;

                application.Status = newStatus;
                _context.ApplicationStatusHistories.Add(new ApplicationStatusHistory
                {
                    ApplicationId = application.Id,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    ChangedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }


        public async Task<bool> IncrementInterviewCountAsync(int jobRequisitionId, int candidateId)
        {
            try
            {
                // 1. Find the parent Application record.
                var application = await FindApplicationAsync(jobRequisitionId, candidateId);
                if (application == null)
                {
                    return false;
                }

                // 2. Find the Interview record associated with this application.
                // We fetch with tracking enabled because we intend to update it.
                var interview = await _context.Interviews
                    .FirstOrDefaultAsync(i => i.ApplicationId == application.Id);

                if (interview == null)
                {
                    // 3a. If no interview record exists, create a new one.
                    ////_logger.LogInformation("No existing interview record found for Application ID {ApplicationId}. Creating a new one.", application.Id);
                    //interview = new Interview
                    //{
                    //    ApplicationId = application.Id,
                    //    InterviewCount = 1, // Start the count at 1
                    //    Status = InterviewStatus.Scheduled, // Example default status
                    //    ScheduledAt = DateTime.UtcNow // Example default time
                    //};
                    //_context.Interviews.Add(interview);
                    Console.WriteLine( "No interview exists ");
                }
                else
                {
                    // 3b. If it already exists, just increment the count.
                    interview.InterviewCount += 1;
                    //_logger.LogInformation("Incrementing interview count for Application ID {ApplicationId}. New count: {Count}", application.Id, interview.InterviewCount);
                }

                // 4. Save the new or updated record to the database.
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error incrementing interview count for JR ID {JR_ID} and Candidate ID {C_ID}", jobRequisitionId, candidateId);
                return false;
            }
        }
    }
}
