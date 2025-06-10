// File Path: Repositories/EvaluationService.cs

using Microsoft.EntityFrameworkCore;
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

            //_context.PanelEvaluationLinks.Add(newLink);
            //await _context.SaveChangesAsync();

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
            var link = await _context.PanelEvaluationLinks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Token == token && l.Status == "PENDING");

            // If the link doesn't exist, is already submitted, or has expired
            if (link == null || (link.ExpiresAt.HasValue && link.ExpiresAt < DateTime.UtcNow))
            {
                _logger.LogWarning("Invalid or expired evaluation token presented: {Token}", token);
                return null; // The controller will interpret this as "not found" or "forbidden".
            }

            // TODO: In a real scenario, you would fetch details from the Interview table
            // using link.InterviewId to get the real candidate name, role, etc.
            // For now, we return placeholder data.
            var formDetails = new EvaluationFormPocDto
            {
                CandidateName = "John Doe (from Token)",
                JobRole = "Senior Software Engineer",
                InterviewLevel = "Technical Round 1",
                InterviewerPrompt = "Please provide your feedback for this candidate."
            };

            return formDetails;
        }

        public async Task<bool> SubmitEvaluationAsync(SubmitEvaluationDto submissionDto)
        {
            // Find the link that is pending and matches the token.
            var link = await _context.PanelEvaluationLinks
                .FirstOrDefaultAsync(l => l.Token == submissionDto.Token && l.Status == "PENDING");

            if (link == null || (link.ExpiresAt.HasValue && link.ExpiresAt < DateTime.UtcNow))
            {
                _logger.LogWarning("Attempted to submit evaluation with an invalid, expired, or already used token: {Token}", submissionDto.Token);
                return false; // Submission failed.
            }

            // 1. Create the response record
            var newResponse = new PanelEvaluationResponse
            {
                PanelEvaluationLinkId = link.Id,
                SubmittedByEmail = submissionDto.SubmittedByEmail,
                Feedback = submissionDto.FeedbackJson, // Storing the raw JSON
                SubmittedAt = DateTime.UtcNow
            };
            _context.PanelEvaluationResponses.Add(newResponse);

            // 2. Update the original link to mark it as submitted
            link.Status = "SUBMITTED";

            // 3. Save both changes in a single transaction
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully submitted evaluation for token {Token} by {Email}", submissionDto.Token, submissionDto.SubmittedByEmail);
            return true;
        }
        public async Task<ViewEvaluationDto> GetSubmittedEvaluationAsync(int interviewId)
        {
            // Find the original link associated with the interview.
            // We need this to potentially get context about the interview.
            var link = await _context.PanelEvaluationLinks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.InterviewId == interviewId);

            if (link == null)
            {
                _logger.LogWarning("No evaluation link found for Interview ID {InterviewId}", interviewId);
                return null;
            }

            // Now find the response that was submitted using this link.
            var response = await _context.PanelEvaluationResponses
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.PanelEvaluationLinkId == link.Id);

            if (response == null)
            {
                _logger.LogWarning("Evaluation for Interview ID {InterviewId} has not been submitted yet.", interviewId);
                return null; // Form exists but hasn't been submitted
            }

            // TODO: In a real scenario, you would fetch details from the Interview table
            // using link.InterviewId to get the real candidate name, role, etc.
            // For now, we return placeholder data mixed with real data.
            var result = new ViewEvaluationDto
            {
                CandidateName = "John Doe (from DB)",
                JobRole = "Senior Software Engineer",
                InterviewLevel = "Technical Round 1",
                SubmittedByEmail = response.SubmittedByEmail,
                SubmittedAt = response.SubmittedAt,
                FeedbackJson = response.Feedback // The raw JSON string from the database
            };

            return result;
        }
    }
}