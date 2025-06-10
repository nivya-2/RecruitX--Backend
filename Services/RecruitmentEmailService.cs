using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitX.Models;

public class RecruitmentEmailService : IRecruitmentEmailService
{
    private readonly AppDbContext _context;
    private readonly IGmailEmailService _emailService;
    private readonly ILogger<RecruitmentEmailService> _logger;

    public RecruitmentEmailService(AppDbContext context, IGmailEmailService emailService, ILogger<RecruitmentEmailService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendScreeningEmailAsync(int applicationId) =>
        await SendEmailFromTemplate(applicationId, "Screening Mail");

    public async Task SendJobOfferEmailAsync(int applicationId) =>
        await SendEmailFromTemplate(applicationId, "Job Offer");

    public async Task SendRejectionEmailAsync(int applicationId) =>
        await SendEmailFromTemplate(applicationId, "Rejection Mail");

    public async Task SendInterviewInvitationAsync(int interviewId, string? meetingLink)
    {
        var interview = await _context.Interviews.FindAsync(interviewId);
        if (interview == null)
            throw new InvalidOperationException("Interview not found.");

        var application = await _context.Applications.FindAsync(interview.ApplicationId);
        if (application == null)
            throw new InvalidOperationException("Associated application not found.");

        var candidate = await _context.Candidates.FindAsync(application.CandidateId);
        var jobDescription = await _context.JobDescriptions.FindAsync(application.JobDescriptionId);
        var job = await _context.JobRequisitions.FindAsync(jobDescription?.JobRequisitionId);

        if (candidate == null || jobDescription == null || job == null)
            throw new InvalidOperationException("Missing related data for interview.");

        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Name == "Interview Invitation Candidate");

        if (template == null)
            throw new InvalidOperationException("Email template 'Interview Invitation Candidate' not found.");

        string meetingLinkDetails = string.IsNullOrWhiteSpace(meetingLink)
            ? string.Empty
            : $"<li><b>Meeting Link:</b> <a href=\"{meetingLink}\">{meetingLink}</a></li>";

        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = candidate.CandidateName,
            ["job_title"] = job.Role,
            ["interview_date"] = interview.ScheduledAt.ToString("MMMM dd, yyyy"),
            ["interview_start_time"] = interview.ScheduledAt.ToString("hh:mm tt"),
            ["interview_end_time"] = interview.ScheduledTo.ToString("hh:mm tt"),
            ["meeting_link_details"] = meetingLinkDetails
        };

        string subject = PopulateTemplate(template.Subject, values);
        string body = PopulateTemplate(template.Body, values);

        await _emailService.SendEmailAsync(candidate.Email, subject, body, isHtml: true);
    }

    private async Task SendEmailFromTemplate(int applicationId, string templateName)
    {
        _logger.LogInformation("Start: SendEmailFromTemplate for Application ID: {AppId}", applicationId);

        try
        {
            var application = await _context.Applications.FindAsync(applicationId);
            if (application == null)
                throw new InvalidOperationException($"Application {applicationId} not found.");

            var candidate = await _context.Candidates.FindAsync(application.CandidateId);
            var jobDescription = await _context.JobDescriptions.FindAsync(application.JobDescriptionId);
            var job = await _context.JobRequisitions.FindAsync(jobDescription?.JobRequisitionId);

            if (candidate == null || jobDescription == null || job == null)
                throw new InvalidOperationException("Missing associated candidate or job.");

            var template = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Name == templateName);
            if (template == null)
                throw new InvalidOperationException($"Template '{templateName}' not found.");

            var values = new Dictionary<string, string?>
            {
                ["candidate_name"] = candidate.CandidateName,
                ["job_title"] = job.Role
            };

            string subject = PopulateTemplate(template.Subject, values);
            string body = PopulateTemplate(template.Body, values);

            await _emailService.SendEmailAsync(candidate.Email, subject, body, isHtml: true);

            _logger.LogInformation("Successfully sent '{TemplateName}' email to {Email} with subject '{Subject}'",
                        templateName,
                        candidate.Email,
                        subject);
                }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendEmailFromTemplate for Application ID: {AppId}", applicationId);
            throw; // rethrow so upstream Task.WhenAll can also fail and be logged
        }
    }


    private string PopulateTemplate(string template, Dictionary<string, string?> values)
    {
        foreach (var kvp in values)
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value ?? string.Empty);

        return template;
    }
}
