using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using RecruitX.Controllers.RequestDTOs; // Make sure this using statement is correct
using Microsoft.EntityFrameworkCore;
using RecruitX.Models; // Assuming models are in this namespace

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IGmailEmailService _emailService;
    private readonly AppDbContext _context;
    private readonly IRecruitmentEmailService _recruitmentEmailService;


    public EmailController(IGmailEmailService emailService, AppDbContext context, IRecruitmentEmailService recruitmentEmailService)
    {
        _emailService = emailService;
        _context = context;
        _recruitmentEmailService = recruitmentEmailService;

    }

    [HttpPost("send-test")]
    public async Task<IActionResult> SendTestEmail([FromQuery] string to)
    {
        var subject = "Notification: Test Email Sent via Gmail API";
        var body = @"Hello,

This is a test email sent using the Gmail API with application permissions.
It confirms that the email sending functionality is working correctly from our backend service.

If you received this email, the integration with Gmail API for sending emails is successful.

Best regards,
RecruitX";

        await _emailService.SendEmailAsync(to, subject, body, isHtml: false);
        return Ok("Email sent successfully.");
    }

    #region Recruitment Email Endpoints

    [HttpPost("send-screening")]
    public async Task<IActionResult> SendScreeningEmail([FromBody] ApplicationEmailRequest request)
    {
        return await SendEmailFromTemplate(request.ApplicationId, "Screening Mail", "Screening email sent.");
    }

    [HttpPost("send-job-offer")]
    public async Task<IActionResult> SendJobOfferEmail([FromBody] ApplicationEmailRequest request)
    {
        return await SendEmailFromTemplate(request.ApplicationId, "Job Offer", "Job Offer email sent.");
    }

    [HttpPost("send-rejection")]
    public async Task<IActionResult> RejectionEmail([FromBody] ApplicationEmailRequest request)
    {
        return await SendEmailFromTemplate(request.ApplicationId, "Rejection Mail", "Rejection email sent.");
    }

    [HttpPost("send-interview-invitation")]
    public async Task<IActionResult> SendInterviewInvitation([FromBody] SendInterviewInvitationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Fetch interview and all related data in one efficient query
        var interview = await _context.Interviews
            .Include(i => i.Application.Candidate)
            .Include(i => i.Application.JobDescription.JobRequisition) // Eager load everything needed
            .FirstOrDefaultAsync(i => i.Id == request.InterviewId);

        if (interview == null)
            return NotFound("Interview not found.");

        // Safely access related data from the single query result
        var candidate = interview.Application?.Candidate;
        // This line is redundant because of the .Include() above, but kept to follow your format.
        // The data is already loaded into interview.Application.JobDescription.JobRequisition
        var job = await _context.JobRequisitions.FindAsync(interview.Application.JobDescription.JobRequisitionId);

        if (candidate == null || job == null)
            return NotFound("Associated application, candidate, or job requisition not found for this interview.");

        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Name == "Interview Invitation Candidate");

        if (template == null)
            return NotFound("Email template 'Interview Invitation Candidate' not found.");

        // --- Start: Corrected logic for handling the meeting link ---

        // 1. Initialize the details string as empty.
        string meetingLinkDetails = string.Empty;

        // 2. CRITICAL: Only populate the string IF a meeting link is provided.
        if (!string.IsNullOrWhiteSpace(request.MeetingLink))
        {
            // Format it nicely for an HTML email.
            meetingLinkDetails = $"<li><b>Meeting Link:</b> <a href=\"{request.MeetingLink}\">{request.MeetingLink}</li></p>";
        }

        // --- End: Corrected logic ---

        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = candidate.CandidateName,
            ["job_title"] = job.Role,
            ["interview_date"] = interview.ScheduledAt.ToString("MMMM dd, yyyy"),
            ["interview_start_time"] = interview.ScheduledAt.ToString("hh:mm tt"),
            ["interview_end_time"] = interview.ScheduledTo.ToString("hh:mm tt"),
            // This key MUST EXACTLY match the placeholder in your email template.
            ["meeting_link_details"] = meetingLinkDetails
        };

        var subject = PopulateTemplate(template.Subject, values);
        var body = PopulateTemplate(template.Body, values);

        await _emailService.SendEmailAsync(candidate.Email, subject, body, isHtml: true);

        return Ok("Interview invitation email sent.");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// A generic helper method to send emails based on an ApplicationId and a template name.
    /// This reduces code duplication across screening, offer, and rejection endpoints.
    /// </summary>
    private async Task<IActionResult> SendEmailFromTemplate(int applicationId, string templateName, string successMessage)
    {
        // Fetch the application and its related entities in one go.
        var application = await _context.Applications
            .Include(a => a.Candidate)
            .Include(a => a.JobDescription)
                .ThenInclude(jd => jd.JobRequisition)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
            return NotFound($"Application with ID {applicationId} not found.");

        var candidate = application.Candidate;
        var job = await _context.JobRequisitions.FindAsync(application.JobDescription.JobRequisitionId);

        if (candidate == null || job == null)
            return NotFound("Associated candidate or job requisition not found for this application.");

        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Name == templateName);

        if (template == null)
            return NotFound($"Email template '{templateName}' not found.");

        // Prepare placeholder values
        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = candidate.CandidateName,
            ["job_title"] = job.Role
            // Add more placeholders here if needed for specific templates
        };

        string subject = PopulateTemplate(template.Subject, values);
        string body = PopulateTemplate(template.Body, values);

        await _emailService.SendEmailAsync(candidate.Email, subject, body, isHtml: true);

        return Ok(successMessage);
    }

    private string PopulateTemplate(string template, Dictionary<string, string?> values)
    {
        if (string.IsNullOrEmpty(template) || values == null) return template ?? string.Empty;

        foreach (var kvp in values)
        {
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value ?? string.Empty);
        }

        return template;
    }

    #endregion
}