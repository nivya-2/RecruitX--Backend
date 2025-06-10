using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models.IdentityGovernance;
using RecruitX.AI;
using RecruitX.Data;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;
using static RecruitX.Controllers.ApplicationDetailsDTO;

namespace RecruitX.Repositories
{
    public class JobDescriptionService : IJobDescriptionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JobDescriptionService> _logger;
        private readonly GeminiJobDescriptionGenerator _gemini;

        private static readonly List<ApplicationStatus> Workflow = Enum.GetValues<ApplicationStatus>()
          .Where(s => s != ApplicationStatus.Rejected) // Exclude Rejected from the standard path
          .OrderBy(s => (int)s)
          .ToList();


        public JobDescriptionService(AppDbContext context, ILogger<JobDescriptionService> logger, GeminiJobDescriptionGenerator gemini)
        {
            _context = context;
            _logger = logger;
            _gemini = gemini;

        }

        public async Task<JobDescriptionDTO> GenerateJobDescriptionFromRequisitionAsync(int jobRequisitionId)
        {
            var jr = await _context.JobRequisitions
                .Include(j => j.Location)
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .FirstOrDefaultAsync(j => j.Id == jobRequisitionId);

            if (jr == null)
                return null;

            var dto = new JobDescriptionDTO
            {
                JobRequisitionId = jr.Id,
                Role = jr.Role,
                WorkLocation = $"{jr.Location?.LocationName}, {jr.Location?.Country}",
                TotalExperienceYears = jr.TotalExperienceYears,
                TotalExperienceMonths = 0,
                RelevantExperienceYears = jr.RelevantExperienceYears,
                RelevantExperienceMonths = 0,
                Qualification = jr.Qualification,
                JobPurpose = jr.JobPurpose,
                JobSpecification = jr.JobSpecification,
                OnboardingDate = jr.ExpectedOnboardingDate?.ToString("dd/MM/yyyy"),
                JobDescription = jr.JobDuties,

            };

            var skills = jr.JobSkills.Select(js => new
            {
                js.Skill.SkillName,
                js.SkillType
            });

            dto.SkillsMandatory = string.Join(", ", skills.Where(s => s.SkillType.ToString() == "Mandatory").Select(s => s.SkillName));
            dto.SkillsPrimary = string.Join(", ", skills.Where(s => s.SkillType.ToString() == "Primary").Select(s => s.SkillName));
            dto.SkillsGood = string.Join(", ", skills.Where(s => s.SkillType.ToString() == "Good").Select(s => s.SkillName));

            var prompt = $"""
You are an expert HR professional creating a compelling job description. Generate a comprehensive, professional job posting for the following position:

**Position Details:**
- Role: {dto.Role}
- Required Experience: {dto.TotalExperienceYears} years
- Educational Requirements: {dto.Qualification}

**Skills & Competencies:**
- Essential Skills (Must Have): {dto.SkillsMandatory}
- Core Skills (Preferred): {dto.SkillsPrimary}  
- Additional Skills (Nice to Have): {dto.SkillsGood}

**Role Information:**
- Purpose/Objective: {dto.JobPurpose}
- Key Specifications: {dto.JobSpecification}
- Primary Responsibilities: {dto.JobDescription}

**Output Requirements:**
Create a job description with ONLY these sections (do not add extra sections like Department, Reports To, or Compensation):

1. **Job Title & Summary**: Start with the role title, followed by 2-3 compelling sentences about the role's impact and what makes it exciting
2. **Key Responsibilities**: 6-8 bullet points using strong action verbs, focusing on outcomes and value delivered
3. **Required Qualifications**: Clearly separate education, experience, and technical requirements
4. **Technical Skills**: Present skills in the exact priority structure provided:
   - **Essential/Mandatory**: [List mandatory skills]
   - **Primary/Preferred**: [List primary skills]  
   - **Good to Have/Nice to Have**: [List additional skills]
5. **What You'll Gain**: Brief mention of growth opportunities, learning, or impact (2-3 sentences)

**Critical Instructions:**
- Do NOT add sections for Department, Reports To, Compensation, or Benefits
- Do NOT use placeholder text like "(Add details here)" or "(experience a plus)"
- Do NOT mention specific company names, products, or platforms (like RecruitX)
- Keep all content generic and company-agnostic
- ONLY use the exact skills provided in the DTO - do NOT add or invent additional skills
- Skills categorization MUST match exactly:
  * Essential/Mandatory: ONLY {dto.SkillsMandatory}
  * Primary/Preferred: ONLY {dto.SkillsPrimary}
  * Good to Have: ONLY {dto.SkillsGood}
- The Job Purpose ({dto.JobPurpose}) must be the central theme throughout
- Job Specification ({dto.JobSpecification}) should directly inform the requirements section
- Do NOT add specific technology versions unless provided in the DTO
- Keep responsibilities focused and concise - avoid generic software development tasks
- Write in a way that could apply to any technology company

**Style Guidelines:**
- Use active voice and impactful action verbs (develop, architect, optimize, lead, etc.)
- Focus on what the candidate will accomplish, not just what they'll do
- Make technical requirements specific and measurable
- Ensure the description flows naturally and tells a story about the role
""";
            try
            {
                dto.JobDescription = await _gemini.GenerateJobDescriptionAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Gemini API failed to generate job description");
                dto.JobDescription = "AI generation failed. Please write manually.";
            }

            return dto;
        }

        public async Task<bool> SaveDraftJobDescriptionAsync(JobDescriptionDTO dto, string userEmail)
        {
            var jr = await _context.JobRequisitions.FirstOrDefaultAsync(j => j.Id == dto.JobRequisitionId);
            if (jr == null)
                return false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return false;

            var existingJD = await _context.JobDescriptions
                .FirstOrDefaultAsync(j => j.JobRequisitionId == dto.JobRequisitionId);

            if (existingJD == null)
            {
                var jd = new JobDescription
                {
                    JobRequisitionId = dto.JobRequisitionId,
                    JobDesc = dto.JobDescription,
                    Updates = dto.AdditionalInfo,
                    CreatedBy = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _context.JobDescriptions.Add(jd);
            }
            else
            {
                existingJD.JobDesc = dto.JobDescription;
                existingJD.Updates = dto.AdditionalInfo;
            }

            jr.JDstatus = Status.Draft;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubmitJobDescriptionAsync(JobDescriptionDTO dto, string userEmail)
        {
            var jr = await _context.JobRequisitions.FirstOrDefaultAsync(j => j.Id == dto.JobRequisitionId);
            if (jr == null)
                return false;

            var existingJD = await _context.JobDescriptions
                .FirstOrDefaultAsync(j => j.JobRequisitionId == dto.JobRequisitionId);

            if (existingJD == null)
                return false; // Can't submit if draft doesn't exist

            existingJD.JobDesc = dto.JobDescription;
            existingJD.Updates = dto.AdditionalInfo;

            jr.JDstatus = Status.Generated;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<TrackJdDTO>> GetJobDescriptionsForUserAsync(string userEmail)
        {
            try
            {
                _logger.LogInformation("Fetching job descriptions for user email: {UserEmail}", userEmail);

                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("User with email {UserEmail} not found.", userEmail);
                    return Enumerable.Empty<TrackJdDTO>();
                }

                var userId = user.Id;

                // Query JrAssignments where AssignedTo == current user and JDstatus is NOT Generated (for example)
                var jobDescriptions = await _context.JrAssignments
                    .Where(assign => assign.AssignedTo == userId && assign.JobRequisition.JDstatus == Status.Generated)
                    .Join(
                        _context.JobDescriptions,
                        assign => assign.JobRequisitionId,
                        jd => jd.JobRequisitionId,
                        (assign, jd) => new { assign, jd }
                    )
                    .Select(result => new TrackJdDTO
                    {
                        JobRequisitionId = result.jd.JobRequisitionId,
                        RoleTitle = result.assign.JobRequisition.Role,
                        BusinessUnit = result.assign.JobRequisition.Department != null
                            ? result.assign.JobRequisition.Department.Name
                            : "N/A",
                        CreatedDate = DateOnly.FromDateTime(result.jd.CreatedAt),
                        JobStatus = result.assign.JobRequisition.JrStatus,
                        FilledPositions = result.jd.FilledPositions,
                        NumberOfPositions = result.assign.JobRequisition.NumPositions
           
            }).OrderByDescending(jr => jr.CreatedDate)

                    .ToListAsync();

                return jobDescriptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job descriptions for user email {UserEmail}", userEmail);
                throw;
            }
        }


        public async Task<IEnumerable<PendingJdDTO>> GetPendingJdsForUserAsync(string userEmail)
        {
            try
            {
                _logger.LogInformation("Fetching pending JDs for user email: {UserEmail}", userEmail);

                // Step 1: Get user
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("User with email {UserEmail} not found.", userEmail);
                    return Enumerable.Empty<PendingJdDTO>();
                }

                var userId = user.Id;

                // Step 2: Get assigned JR IDs
                var userAssignments = await _context.JrAssignments
                    .Where(a => a.AssignedTo == userId)
                    .Select(a => a.JobRequisitionId)
                    .Distinct()
                    .ToListAsync();

                if (!userAssignments.Any())
                {
                    _logger.LogInformation("No JR assignments found for user {UserId}", userId);
                    return Enumerable.Empty<PendingJdDTO>();
                }

                // Step 3: Get JRs that already have JDs


                // Step 4: Fetch JRs assigned to user that DO NOT have a JD
                var pendingJds = await _context.JobRequisitions
                                .Where(jr => userAssignments.Contains(jr.Id) &&
                                 jr.JDstatus != Status.Generated)
                                 .Include(jr => jr.Department)
                                 .Include(jr => jr.HiringManagerEmployee)
                                 .Include(jr => jr.Location)
                                 .Select(jr => new PendingJdDTO
                                 {
                                     JobRequisitionId = jr.Id,
                                     RoleTitle = jr.Role,
                                     BusinessUnit = jr.Department != null ? jr.Department.Name : "N/A",
                                     location = jr.Location != null ? jr.Location.LocationName : "N/A",
                                     openPositions = jr.NumPositions ,
                                     Actions = new List<string> { jr.JDstatus.ToString() },
                                     HiringManager = jr.HiringManagerEmployee != null ?
            $"{jr.HiringManagerEmployee.FirstName} {jr.HiringManagerEmployee.LastName}" : "N/A",
                                     CreatedDate = DateOnly.FromDateTime(jr.CreatedAt),
                                     //JobStatus = jr.JDstatus.G // Or jr.JdStatus if needed
                                 })
                                                 .OrderByDescending(jr => jr.CreatedDate)

    .ToListAsync();

                //_logger.LogInformation("Found {Count} pending JDs for user {UserId}", pendingJds.Count, userId);
                return pendingJds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending JDs for user {UserEmail}", userEmail);
                throw;
            }
        }

        public async Task<JobDescriptionDTO> GetJobDescriptionDetailsAsync(int jobRequisitionId)
        {
            var jobDetails = await _context.JobRequisitions
                .Include(jr => jr.Location)
                .Include(jr => jr.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Join(_context.JobDescriptions,
                    jr => jr.Id,
                    jd => jd.JobRequisitionId,
                    (jr, jd) => new { jr, jd })
                .Where(x => x.jr.Id == jobRequisitionId)
                .Select(x => new JobDescriptionDTO
                {
                    JobRequisitionId = x.jr.Id,
                    JobDescriptionId = x.jd.Id,

                    Role = x.jr.Role,
                    WorkLocation = x.jr.Location != null
                        ? $"{x.jr.Location.LocationName}, {x.jr.Location.Country}"
                        : "Not Specified",

                    TotalExperienceYears = x.jr.TotalExperienceYears,
                    TotalExperienceMonths = 0,
                    RelevantExperienceYears = x.jr.RelevantExperienceYears,
                    RelevantExperienceMonths = 0,

                    Qualification = x.jr.Qualification,

                    SkillsMandatory = string.Join(", ", x.jr.JobSkills
                        .Where(s => s.SkillType == SkillTypes.Mandatory)
                        .Select(s => s.Skill.SkillName)),

                    SkillsPrimary = string.Join(", ", x.jr.JobSkills
                        .Where(s => s.SkillType == SkillTypes.Primary)
                        .Select(s => s.Skill.SkillName)),

                    SkillsGood = string.Join(", ", x.jr.JobSkills
                        .Where(s => s.SkillType == SkillTypes.GoodToHave)
                        .Select(s => s.Skill.SkillName)),

                    JobPurpose = x.jr.JobPurpose,
                    JobSpecification = x.jr.JobSpecification,
                    JobDescription = x.jd.JobDesc,
                    AdditionalInfo = x.jd.Updates,

                    OnboardingDate = x.jr.ExpectedOnboardingDate.HasValue
                        ? x.jr.ExpectedOnboardingDate.Value.ToString("dd/MM/yyyy")
                        : string.Empty


                })
                .FirstOrDefaultAsync();

            return jobDetails;
        }

        public async Task<IEnumerable<JdApplicantsDTO>> GetApplicantsForJdAsync(int jobDescriptionId)
        {
            // First, ensure the JobDescriptionId is valid or exists (optional step,
            // the query below will just return empty if jdId doesn't match anything)
            // bool jdExists = await _context.JobDescriptions.AnyAsync(jd => jd.Id == jobDescriptionId);
            // if (!jdExists) {
            //     return Enumerable.Empty<JdApplicantsDTO>(); // Or throw NotFoundException
            // }

            var applicantsDto = await _context.Applications
                .Where(app => app.JobDescriptionId == jobDescriptionId && app.Candidate != null) // Ensure candidate is not null
                .Select(app => new JdApplicantsDTO
                {
                    CandidateId = app.Candidate.Id,
                    CandidateName = app.Candidate.CandidateName,
                    CandidateEmail = app.Candidate.Email,
                    CandidatePhone = app.Candidate.ContactNumber,
                    // Assuming Candidate.TotalExperienceYears is 'short' or can be safely cast to 'short'.
                    // If Candidate.TotalExperienceYears is int, you might need a cast: (short)app.Candidate.TotalExperienceYears
                    TotalExperienceYears = app.Candidate.TotalExperienceYears,
                    Source = app.Candidate.Source,
                    ApplicationID = app.Id

                    // The 'Actions' property is initialized by the JdApplicantsDTO constructor
                })
                

                .ToListAsync();

            return applicantsDto;
        }

        public async Task<CandidateDetailsDTO?> GetCandidateDetailsByApplicationIdAsync(int applicationId)
        {
            var application = await _context.Applications
        .AsNoTracking()
        .Include(a => a.Candidate)
            .ThenInclude(c => c.CurrentLocation)
            .Include(a => a.JobDescription)
        .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null || application.Candidate == null)
                return null;

            var candidate = application.Candidate;
            var jobDescription = application.JobDescription;

                JobRequisition? jobRequisition = null;
                if (jobDescription != null)
                {
                    // Query 2: Fetch the JobRequisition separately using the FK from the JobDescription.
                    jobRequisition = await _context.JobRequisitions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(jr => jr.Id == jobDescription.JobRequisitionId);
                }

            return new CandidateDetailsDTO
            {
                CandidateID = candidate.Id,
                CandidateName = candidate.CandidateName,
                CandidatePhone = candidate.ContactNumber,
                CandidateEmail = candidate.Email,
                TotalExperience = (short)candidate.TotalExperienceYears,
                RelavantExperience = (short)candidate.RelevantExperienceYears,
                NoticePeriod = candidate.NoticePeriodDays,
                CurrentCTC = candidate.CurrentCTC,
                ExpectedCTC = application.ExpectedCTC,
                Source = candidate.Source,
                CurrentLocation = candidate.CurrentLocation?.LocationName ?? "N/A",
                CurrentEmployer = candidate.CurrentEmployer,
                Status = application.Status.ToString(),
                ApplicationID = application.Id,
                JrStatus = jobRequisition?.JrStatus.ToString() ?? "Unknown",

            };

            
        }

        public async Task<ApplicationDetailsPageDTO?> GetApplicationPageDetailsAsync(int applicationId)
        {
            // 1. Reuse your existing method to get candidate info
            var candidateInfo = await GetCandidateDetailsByApplicationIdAsync(applicationId);
            if (candidateInfo == null)
            {
                return null; // Application or candidate not found
            }

            // 2. Get the status history for the timeline
            var statusHistory = await _context.ApplicationStatusHistories
                .AsNoTracking()
                .Where(h => h.ApplicationId == applicationId)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();

            // 3. Build the timeline steps
            var timelineSteps = new List<ApplicationTimelineStepDto>();
            Enum.TryParse<ApplicationStatus>(candidateInfo.Status, out var currentStatusEnum);
            int currentStatusIndex = Workflow.IndexOf(currentStatusEnum);
            //foreach (var stage in Workflow)
            //{
            //    int stageIndex = Workflow.IndexOf(stage);

            //    bool isCompleted = (currentStatusIndex >= stageIndex) || (stage == ApplicationStatus.Applied);

            //    var historyEntry = statusHistory.FirstOrDefault(h => h.NewStatus == stage);

            //    timelineSteps.Add(new ApplicationTimelineStepDto
            //    {
            //        Label = Regex.Replace(stage.ToString(), "(\\B[A-Z])", " $1"),
            //        Completed = isCompleted,

            //        // --- FIX #2: Change the date format to dd-MM-yyyy ---
            //        Date = historyEntry?.ChangedAt.ToString("dd-MM-yyyy")
            //    });
            //}
            foreach (var stage in Workflow)
            {
                // Find if a history record exists for this specific stage.
                var historyEntry = statusHistory.FirstOrDefault(h => h.NewStatus == stage);

                // This is the new, robust logic:
                // A stage is "completed" if a history record for it exists in the database.
                // We also keep your business rule that 'Applied' is always complete.
                bool isCompleted = (historyEntry != null) || (stage == ApplicationStatus.Applied);

                timelineSteps.Add(new ApplicationTimelineStepDto
                {
                    Label = Regex.Replace(stage.ToString(), "(\\B[A-Z])", " $1"),
                    Completed = isCompleted, // Use our new, correct boolean
                    Date = historyEntry?.ChangedAt.ToString("dd-MM-yyyy")
                });
            }

            // 4. Assemble and return the final DTO
            return new ApplicationDetailsPageDTO
            {
                CandidateInfo = candidateInfo,
                StatusTimeline = timelineSteps,
                IsProcessFinished = candidateInfo.Status == nameof(ApplicationStatus.Joined) || candidateInfo.Status == nameof(ApplicationStatus.Rejected)
            };
        }

        public async Task<bool> UpdateApplicationStatusAsync(int applicationId, string action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var application = await _context.Applications.FindAsync(applicationId);
                if (application == null || application.Status == ApplicationStatus.Joined || application.Status == ApplicationStatus.Rejected)
                {
                    return false;
                }

                // Prevent action on a finished application
                if (application.Status == ApplicationStatus.Joined || application.Status == ApplicationStatus.Rejected)
                {
                    return false;
                }

                var oldStatus = application.Status;
                ApplicationStatus newStatus;

                if (action.Equals("progress", StringComparison.OrdinalIgnoreCase))
                {
                    int currentIndex = Workflow.IndexOf(oldStatus);
                    if (currentIndex < 0 || currentIndex >= Workflow.Count - 1)
                    {
                        return false; // Cannot progress from an unknown or final state
                    }
                    newStatus = Workflow[currentIndex + 1];
                }
                else if (action.Equals("reject", StringComparison.OrdinalIgnoreCase))
                {
                    newStatus = ApplicationStatus.Rejected;
                }
                else
                {
                    return false; // Invalid action
                }
                if (newStatus == ApplicationStatus.Joined)
                {
                    var jobDescription = await _context.JobDescriptions.FindAsync(application.JobDescriptionId);
                    JobRequisition? jobRequisition = null;
                    if (jobDescription != null)
                    {
                        // Query 2: Fetch the JobRequisition separately using the FK from the JobDescription.
                        jobRequisition = await _context.JobRequisitions
                            .FirstOrDefaultAsync(jr => jr.Id == jobDescription.JobRequisitionId);
                    }
                    if (jobDescription != null)
                    {
                        // Increment the counter
                        jobDescription.FilledPositions += 1;
                        _logger.LogInformation(
                            "Incremented FilledPositions for JD ID {JobDescriptionId} to {NewCount} for Application ID {ApplicationId}",
                            jobDescription.Id, jobDescription.FilledPositions, applicationId);
                        if (jobRequisition != null)
                        {
                            // Using '>=' is a safeguard in case the counts ever get out of sync.
                            if (jobDescription.FilledPositions >= jobRequisition.NumPositions)
                            {
                                // 3. If full, update the Job Requisition status to Closed.
                                jobRequisition.JrStatus = JobStatus.Closed;
                                _logger.LogInformation(
                                    "All positions filled for JR ID {JobRequisitionId}. Status automatically changed to Closed.",
                                    jobRequisition.Id);
                            }
                        }
                    }
                    else
                    {
                        // This case indicates a data integrity issue but we shouldn't fail the whole transaction.
                        // Just log a warning that the JD could not be found.
                        _logger.LogWarning(
                            "Attempted to increment FilledPositions for a joined candidate (Application ID: {ApplicationId}), but could not find the associated JobDescription with ID: {JobDescriptionId}",
                            applicationId, application.JobDescriptionId);
                    }
                }
                application.Status = newStatus;
                _context.ApplicationStatusHistories.Add(new ApplicationStatusHistory
                {
                    ApplicationId = applicationId,
                    OldStatus = oldStatus, // Assign the 'ApplicationStatus?' enum directly
                    NewStatus = newStatus, // Assign the 'ApplicationStatus' enum directly
                    ChangedAt = DateTime.UtcNow,
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // If any error occurred, log it and roll back the transaction
                _logger.LogError(ex, "An error occurred while updating application status for ID {ApplicationId}. Transaction rolled back.", applicationId);
                await transaction.RollbackAsync();
                return false;
            }
        }


        public async Task<BulkAddResultDTO> BulkAddCandidatesAsync(int jobRequisitionId, List<CandidateDetailsDTO> candidates, string createdByUserEmail)
        {
            var result = new BulkAddResultDTO();

            var createdByUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.ToLower() == createdByUserEmail.ToLower());
            if (createdByUser == null)
            {
                result.FailureCount = candidates.Count;
                result.FailureMessages.Add($"The user '{createdByUserEmail}' was not found.");
                return result;
            }

            var jobDescription = await _context.JobDescriptions.AsNoTracking().FirstOrDefaultAsync(jd => jd.JobRequisitionId == jobRequisitionId);
            if (jobDescription == null)
            {
                result.FailureCount = candidates.Count;
                result.FailureMessages.Add($"The job with Requisition ID {jobRequisitionId} does not exist.");
                return result;
            }

            var emailsToFind = candidates.Select(c => c.CandidateEmail.ToLower()).ToHashSet();
            var locationStringsToFind = candidates.Select(c => c.CurrentLocation).Concat(candidates.Select(c => c.preferedLocation)).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.ToLower()).ToHashSet();
            var skillNamesToFind = candidates.SelectMany(c => (c.skill ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).Select(s => s.ToLower()).ToHashSet();

            var existingCandidates = await _context.Candidates.Where(c => emailsToFind.Contains(c.Email.ToLower())).ToDictionaryAsync(c => c.Email.ToLower(), c => c);
            var existingLocations = await _context.Locations.Where(l => locationStringsToFind.Contains(l.LocationName.ToLower())).ToDictionaryAsync(l => l.LocationName.ToLower(), l => l);
            var existingSkills = await _context.Skills.Where(s => skillNamesToFind.Contains(s.SkillName.ToLower())).ToDictionaryAsync(s => s.SkillName.ToLower(), s => s);

            foreach (var dto in candidates)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var currentLocation = await GetOrCreateLocationAsync(dto.CurrentLocation, existingLocations);
                    var preferredLocation = await GetOrCreateLocationAsync(dto.preferedLocation, existingLocations);

                    if (!existingCandidates.TryGetValue(dto.CandidateEmail.ToLower(), out var candidate))
                    {
                        candidate = new Candidate { CreatedAt = DateTime.UtcNow };
                        _context.Candidates.Add(candidate);
                        existingCandidates[dto.CandidateEmail.ToLower()] = candidate;
                    }

                    // --- MAPPING SECTION (Carefully completed) ---
                    candidate.Email = dto.CandidateEmail;
                    candidate.CandidateName = dto.CandidateName;
                    candidate.ContactNumber = dto.CandidatePhone;
                    candidate.TotalExperienceYears = (short)dto.TotalExperience;
                    candidate.TotalExperienceMonths = 0;
                    candidate.RelevantExperienceYears = (short)dto.RelavantExperience;
                    candidate.RelevantExperienceMonths = 0;
                    candidate.CurrentEmployer = dto.CurrentEmployer;
                    candidate.CurrentCTC = dto.CurrentCTC;
                    candidate.NoticePeriodDays = dto.NoticePeriod;
                    candidate.LinkedinUrl = dto.linkedin;
                    candidate.CurrentLocation = currentLocation;
                    candidate.PreferredLocation = preferredLocation;
                    candidate.Source = dto.Source;
                    candidate.SubSource = dto.subSource;
                    candidate.ProposedRole = !string.IsNullOrWhiteSpace(dto.role) ? dto.role : "Not specified";
                    candidate.UpdatedAt = DateTime.UtcNow;


                    var hasApplication = await _context.Applications
    .AsNoTracking()
    .AnyAsync(app => app.CandidateId == candidate.Id && app.JobDescriptionId == jobDescription.Id);

                    if (hasApplication)
                    {
                        result.FailureCount++;
                        result.FailureMessages.Add($"Application for candidate {dto.CandidateEmail} already exists for requisition {jobRequisitionId}.");
                        await transaction.RollbackAsync();
                        continue; // Skip to next candidate
                    }

                    var application = new Application
                    {
                        Candidate = candidate,
                        JobDescriptionId = jobDescription.Id,
                        Status = ApplicationStatus.TechnicalInterview,
                        ExperienceYears = dto.TotalExperience,
                        ExperienceMonths = 0,
                        ExpectedCTC = dto.ExpectedCTC,
                        CreatedBy = createdByUser.Id,
                        SubmittedOn = DateTime.UtcNow
                    };
                    _context.Applications.Add(application);

                    var initialHistoryRecord = new ApplicationStatusHistory
                    {
                        Application = application,
                        OldStatus = null,
                        NewStatus = ApplicationStatus.TechnicalInterview,
                        ChangedAt = (DateTime)application.SubmittedOn,
                        ChangedBy = createdByUser.Id
                    };
                    _context.ApplicationStatusHistories.Add(initialHistoryRecord);

                    if (!string.IsNullOrWhiteSpace(dto.skill))
                    {
                        var skillNames = dto.skill
                            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(s => s.ToLower())
                            .ToList();

                        var alreadyLinkedSkillNames = new HashSet<string>();

                        // Extract skills already tracked in EF (Application just added, so we rely on in-memory)
                        foreach (var skillEntry in _context.ApplicationSkills.Local.Where(a => a.Application == application))
                        {
                            alreadyLinkedSkillNames.Add(skillEntry.Skill.SkillName.ToLower());
                        }

                        foreach (var skillName in skillNames)
                        {
                            if (!existingSkills.TryGetValue(skillName, out var skill))
                            {
                                skill = new Skill { SkillName = skillName };
                                _context.Skills.Add(skill);
                                existingSkills[skillName] = skill;
                            }

                            // Prevent duplicate ApplicationSkill
                            if (alreadyLinkedSkillNames.Contains(skill.SkillName.ToLower()))
                                continue;

                            _context.ApplicationSkills.Add(new ApplicationSkill
                            {
                                Application = application,
                                Skill = skill
                            });

                            alreadyLinkedSkillNames.Add(skill.SkillName.ToLower());
                        }
                    }


                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to process candidate {CandidateName} ({CandidateEmail})", dto.CandidateName, dto.CandidateEmail);
                    result.FailureCount++;
                    result.FailureMessages.Add($"Failed to process {dto.CandidateName}: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            return result;
        }
        private async Task<Location?> GetOrCreateLocationAsync(string? locationString, Dictionary<string, Location> cache)
        {
            if (string.IsNullOrWhiteSpace(locationString)) return null;

            if (cache.TryGetValue(locationString.ToLower(), out var location))
            {
                return location;
            }

            var parts = locationString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var locationName = parts.Length > 0 ? parts[0] : null;
            if (string.IsNullOrEmpty(locationName)) return null;

            // This should not happen if our pre-fetch is correct, but as a safeguard:
            var dbLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationName.ToLower() == locationName.ToLower());
            if (dbLocation != null)
            {
                cache[locationString.ToLower()] = dbLocation;
                return dbLocation;
            }

            var country = parts.Length > 1 ? parts[1] : null;
            var newLocation = new Location { LocationName = locationName, Country = country };
            _context.Locations.Add(newLocation);
            cache[locationString.ToLower()] = newLocation; // Add new entity to cache before save
            return newLocation;

        }

    }
}
