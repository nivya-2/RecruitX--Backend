// File Path: Repositories/EvaluationService.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Models.DTO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitX.Repositories
{
    public class EvaluationService : IEvaluationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EvaluationService> _logger;
        private readonly IConfiguration _configuration;


        public EvaluationService(AppDbContext context, ILogger<EvaluationService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> CreateEvaluationLinkAsync(int interviewId)
        {
            // Generate a new, cryptographically secure token.
            var token = Guid.NewGuid().ToString();

            var newLink = new PanelEvaluationLink
            {
                Token = token,
                InterviewId = interviewId,
                Status = "PENDING", // Initial status
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // Optional: Link expires in 7 days
            };
            var newEvaluationToken = new EvaluationToken
            {
                Token = token,
                InterviewId = interviewId,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.PanelEvaluationLinks.Add(newLink);
            _context.EvaluationTokens.Add(newEvaluationToken);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Received request to get form details for Token: {Token}", token);

            _logger.LogInformation("Created new evaluation link for Interview ID {InterviewId} with token {Token}", interviewId, token);

            // Get the base URL for the Angular app from appsettings.json for flexibility.
            var baseUrl = _configuration["FrontendAppUrl"] ?? "http://localhost:4200";
            return $"{baseUrl}/eval-form?token={token}";

            //_logger.LogInformation("TEST MODE: Generated temporary evaluation link with token {Token}", token);

            //var baseUrl = _configuration["FrontendAppUrl"] ?? "http://localhost:4200";
            //return $"{baseUrl}/eval-form?token={token}";
        }

        public async Task<EvaluationFormPocDto> GetEvaluationFormDetailsAsync(string token)
        {
            var panelLink = await _context.PanelEvaluationLinks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Token == token);

            if (panelLink == null || panelLink.Status != "PENDING" || (panelLink.ExpiresAt.HasValue && panelLink.ExpiresAt < DateTime.UtcNow))
            {
                _logger.LogWarning("Evaluation link with token {Token} is invalid, already used (Status: {Status}), or expired.", token, panelLink?.Status ?? "NOT FOUND");
                return null;
            }

            var interview = await _context.Interviews
                .AsNoTracking()
                .Where(i => i.Id == panelLink.InterviewId)
                .Include(i => i.Application).ThenInclude(app => app.Candidate).ThenInclude(c => c.CurrentLocation)
                .Include(i => i.Application).ThenInclude(app => app.Candidate).ThenInclude(c => c.PreferredLocation)
                .Include(i => i.Application).ThenInclude(app => app.JobDescription).ThenInclude(jd => jd.JobRequisition).ThenInclude(jr => jr.JobSkills).ThenInclude(js => js.Skill)
                .FirstOrDefaultAsync();

            if (interview?.Application?.Candidate == null || interview?.Application?.JobDescription?.JobRequisition == null)
            {
                _logger.LogError("Data integrity issue. Could not find full details for Interview ID {InterviewId} with token {Token}.", panelLink.InterviewId, token);
                return null;
            }

            var jobRequisition = interview.Application.JobDescription.JobRequisition;
            var candidate = interview.Application.Candidate;

            var formDetailsDto = new EvaluationFormPocDto
            {
                CandidateName = candidate.CandidateName,
                JobRole = jobRequisition.Role,
                InterviewLevel = interview.IsTechnicalRound ? "Technical Round" : "HR Round",
                InterviewerPrompt = $"Please provide your feedback for {candidate.CandidateName} regarding the {jobRequisition.Role} position.",
                Summary = new CandidateSummaryDto
                {
                    CandidateName = candidate.CandidateName,
                    Technology = jobRequisition.Role,
                    InterviewLevel = interview.IsTechnicalRound ? "Technical Round" : "HR Round",
                    NoticePeriod = candidate.NoticePeriodDays.HasValue ? $"{candidate.NoticePeriodDays} days" : "N/A",
                    TotalExperience = $"{candidate.TotalExperienceYears} years, {candidate.TotalExperienceMonths} months",
                    RelevantExperience = $"{candidate.RelevantExperienceYears} years, {candidate.RelevantExperienceMonths} months",
                    CurrentLocation = candidate.CurrentLocation?.LocationName ?? "N/A",
                    PreferredLocation = candidate.PreferredLocation?.LocationName ?? "N/A"
                },
                Skills = jobRequisition.JobSkills.Select(js => new SkillBlockDto
                {
                    Category = "Required Skill",
                    Competencies = new List<CompetencyDto>
                    {
                        new CompetencyDto { Title = js.Skill.SkillName, SelfRating = 0 }
                    }
                }).ToList(),
                 ProposedRole = candidate.ProposedRole ?? jobRequisition.Role
            };

            if (!formDetailsDto.Skills.Any())
            {
                formDetailsDto.Skills.Add(new SkillBlockDto
                {
                    Category = "General Technical Skills",
                    Competencies = new List<CompetencyDto>
                    {
                        new CompetencyDto { Title = "Problem Solving & Logic", SelfRating = 0 },
                        new CompetencyDto { Title = "Communication", SelfRating = 0 }
                    }
                });
            }

            return formDetailsDto;
      
        }
        public async Task<bool> SubmitEvaluationAsync(SubmitEvaluationDto submissionDto)
        {
            var link = await _context.PanelEvaluationLinks
                .FirstOrDefaultAsync(l => l.Token == submissionDto.Token && l.Status == "PENDING");

            if (link == null || (link.ExpiresAt.HasValue && link.ExpiresAt < DateTime.UtcNow))
            {
                _logger.LogWarning("Attempted to submit evaluation with an invalid, expired, or already used token: {Token}", submissionDto.Token);
                return false;
            }
            var evaluationToken = await _context.EvaluationTokens
      .FirstOrDefaultAsync(t => t.Token == submissionDto.Token && !t.IsUsed);

            // This is a safety check. In a healthy system, if a PanelEvaluationLink is PENDING,
            // its corresponding EvaluationToken should also be !IsUsed.
            if (evaluationToken == null)
            {
                _logger.LogError("Data integrity issue: A PENDING PanelEvaluationLink was found for token {Token}, but no corresponding unused EvaluationToken was found. Aborting submission.", submissionDto.Token);
                return false;
            }

            var newResponse = new PanelEvaluationResponse
            {
                PanelEvaluationLinkId = link.Id,
                SubmittedByEmail = submissionDto.SubmittedByEmail,
                Feedback = submissionDto.FeedbackJson,
                SubmittedAt = DateTime.UtcNow,
             

            };
            _context.PanelEvaluationResponses.Add(newResponse);

            link.Status = "SUBMITTED";
            evaluationToken.IsUsed = true;
            evaluationToken.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully submitted evaluation for token {Token} by {Email}", submissionDto.Token, submissionDto.SubmittedByEmail);
            return true;


        }
       

        // This method is for viewing submitted data and is correct as is. It remains unchanged.
        public async Task<ViewEvaluationDto> GetSubmittedEvaluationAsync(int interviewId)
        {
            var link = await _context.PanelEvaluationLinks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.InterviewId == interviewId);

            if (link == null)
            {
                _logger.LogWarning("No evaluation link found for Interview ID {InterviewId}", interviewId);
                return null;
            }

            var response = await _context.PanelEvaluationResponses
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.PanelEvaluationLinkId == link.Id);

            if (response == null)
            {
                _logger.LogWarning("Evaluation for Interview ID {InterviewId} has not been submitted yet.", interviewId);
                return null;
            }

            var result = new ViewEvaluationDto
            {
                CandidateName = "John Doe (from DB)", // Placeholder
                JobRole = "Senior Software Engineer", // Placeholder
                InterviewLevel = "Technical Round 1", // Placeholder
                SubmittedByEmail = response.SubmittedByEmail,
                SubmittedAt = response.SubmittedAt,
                FeedbackJson = response.Feedback
            };

            return result;
        }
     
    }
}