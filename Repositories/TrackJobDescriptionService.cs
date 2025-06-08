using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitX.AI;
using RecruitX.Data;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;

namespace RecruitX.Repositories
{
    public class TrackJobDescriptionService : ITrackJdService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TrackJobDescriptionService> _logger;
        private readonly GeminiJobDescriptionGenerator _gemini;

        public TrackJobDescriptionService(AppDbContext context, ILogger<TrackJobDescriptionService> logger, GeminiJobDescriptionGenerator gemini)
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
                        JobStatus = result.assign.JobRequisition.JrStatus
                    })
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
        openPositions = jr.NumPositions ?? 0,
        Actions = new List<string> { jr.JDstatus.ToString() },
        HiringManager = jr.HiringManagerEmployee != null ?
            $"{jr.HiringManagerEmployee.FirstName} {jr.HiringManagerEmployee.LastName}" : "N/A",
        CreatedDate = DateOnly.FromDateTime(jr.CreatedAt),
        //JobStatus = jr.JDstatus.G // Or jr.JdStatus if needed
    })
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
                    ApplicationID=app.Id
                    
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
        .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null || application.Candidate == null)
                return null;

            var candidate = application.Candidate;

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
                ApplicationID = application.Id
            };
        }

    }
}
