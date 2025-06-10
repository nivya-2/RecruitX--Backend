using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using RecruitX.Controllers.RequestDTOs;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IGmailEmailService _emailService;
    private readonly AppDbContext _context;

    public EmailController(IGmailEmailService emailService, AppDbContext context)
    {
        _emailService = emailService;
        _context = context;
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

    #region Helper Method
    private string PopulateTemplate(string template, Dictionary<string, string?> values)
    {
        if (values == null) return template;

        foreach (var kvp in values)
        {
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value ?? string.Empty);
        }

        return template;
    }
    #endregion

    #region Recruitment Email Endpoints

    [HttpPost("send-screening")]
    public async Task<IActionResult> SendScreeningEmail([FromBody] SendScreeningEmailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var job = await _context.JobRequisitions.FindAsync(request.JobRequisitionId);
        var candidate = await _context.Candidates.FindAsync(request.CandidateId);
        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Name == "Screening Mail");

        if (job == null || candidate == null || template == null)
            return NotFound("Job, candidate, or email template not found.");

        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = candidate.CandidateName,
            ["job_title"] = job.Role
            // add more if your template has other placeholders
        };

        string subject = PopulateTemplate(template.Subject, values);
        string body = PopulateTemplate(template.Body, values);

        await _emailService.SendEmailAsync(candidate.Email, subject, body, isHtml: true);

        return Ok("Screening email sent.");
    }



    [HttpPost("send-interview-invitation")]
    public async Task<IActionResult> SendInterviewInvitation([FromBody] SendInterviewInvitationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var interview = await _context.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.Candidate)
            .Include(i => i.Application)
                .ThenInclude(a => a.JobDescription)
                    .ThenInclude(jd => jd.JobRequisition)
            .FirstOrDefaultAsync(i => i.Id == request.InterviewId);

        if (interview == null)
            return NotFound("Interview not found.");

        var application = interview.Application;
        if (application == null)
            return NotFound("Application not found for this interview.");

        var candidate = application.Candidate;
        if (candidate == null)
            return NotFound("Candidate not found for this application.");

        var jobDescription = application.JobDescription;
        if (jobDescription == null)
            return NotFound("Job description not found for this application.");

        var job = await _context.JobRequisitions
    .FirstOrDefaultAsync(jr => jr.Id == jobDescription.JobRequisitionId);

        if (job == null)
            return NotFound($"Job requisition not found for JD ID: {jobDescription.Id} (Expected JR ID: {jobDescription.JobRequisitionId}).");
    

        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Name == "Interview Invitation Candidate");

        if (template == null)
            return NotFound("Email template not found.");

        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = candidate.CandidateName,
            ["job_title"] = job.Role,
            ["interview_date"] = interview.ScheduledAt.ToString("MMMM dd, yyyy"),
            ["interview_start_time"] = interview.ScheduledAt.ToString("hh:mm tt"),
            ["interview_end_time"] = interview.ScheduledTo.ToString("hh:mm tt")
        };

        var subject = PopulateTemplate(template.Subject, values);
        var body = PopulateTemplate(template.Body, values);

        await _emailService.SendEmailAsync(candidate.Email, subject, body, isHtml: true);

        return Ok("Interview invitation email sent.");
    }




        [HttpPost("send-job-offer")]
    public async Task<IActionResult> SendJobOfferEmail([FromBody] SendScreeningEmailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var job = await _context.JobRequisitions.FindAsync(request.JobRequisitionId);
        if (job == null)
            return NotFound($"Job Requisition not found for ID: {request.JobRequisitionId}");

        var candidate = await _context.Candidates.FindAsync(request.CandidateId);
        if (candidate == null)
            return NotFound($"Candidate not found for ID: {request.CandidateId}");

        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Name == "Job Offer");
        if (template == null)
            return NotFound("Email template 'Job Offer' not found.");


        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = candidate.CandidateName,
            ["job_title"] = job.Role
            // add more if your template has other placeholders
        };

        string subject = PopulateTemplate(template.Subject, values);
        string body = PopulateTemplate(template.Body, values);

        await _emailService.SendEmailAsync(candidate.Email, subject, body, isHtml: true);

        return Ok("Job Offer email sent.");
    }

        [HttpPost("send-rejection")]
    public async Task<IActionResult> RejectionEmail([FromBody] SendScreeningEmailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var job = await _context.JobRequisitions.FindAsync(request.JobRequisitionId);
        if (job == null)
            return NotFound($"Job Requisition not found for ID: {request.JobRequisitionId}");

        var candidate = await _context.Candidates.FindAsync(request.CandidateId);
        if (candidate == null)
            return NotFound($"Candidate not found for ID: {request.CandidateId}");

        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Name == "Rejection Mail");
        if (template == null)
            return NotFound("Email template 'Job Offer' not found.");


        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = candidate.CandidateName,
            ["job_title"] = job.Role
            // add more if your template has other placeholders
        };

        string subject = PopulateTemplate(template.Subject, values);
        string body = PopulateTemplate(template.Body, values);

        await _emailService.SendEmailAsync(candidate.Email, subject, body, isHtml: true);

        return Ok("Rejection email sent.");
    }

    //        var subject = PopulateTemplate(subjectTemplate, values);
    //        var body = PopulateTemplate(bodyTemplate, values);

    //        await _emailService.SendEmailAsync(request.ToEmail, subject, body, isHtml: true);
    //        return Ok("Rejection email sent.");
    //    }

    //    [HttpPost("send-onboarding")]
    //    public async Task<IActionResult> SendOnboardingEmail([FromBody] SendOnboardingEmailRequest request)
    //    {
    //        if (!ModelState.IsValid)
    //            return BadRequest(ModelState);

    //        const string subjectTemplate = "Welcome to {{company_name}} – Your Onboarding Details";
    //        const string bodyTemplate = @"<p>Dear {{employee_name}},</p>
    //<p>Welcome aboard!</p>
    //<p>We are thrilled to have you join us as a {{role}}. Your start date is {{start_date}}.</p>
    //<p>Please complete the following before your first day:
    //<ul>
    //    <li>Fill out the onboarding documents</li>
    //    <li>Set up your company email account</li>
    //    <li>Complete mandatory training sessions</li>
    //</ul>
    //</p>
    //<p>You will be reporting to {{manager_name}} and your workstation will be at {{desk_location}}.</p>
    //<p>For questions, contact {{hr_contact}} at {{hr_email}}.</p>
    //<p>We look forward to working with you!</p>";

    //        var values = new Dictionary<string, string?>
    //        {
    //            ["company_name"] = request.CompanyName,
    //            ["employee_name"] = request.EmployeeName,
    //            ["role"] = request.Role,
    //            ["start_date"] = request.StartDate,
    //            ["manager_name"] = request.ManagerName,
    //            ["desk_location"] = request.DeskLocation,
    //            ["hr_contact"] = request.HrContact,
    //            ["hr_email"] = request.HrEmail,
    //            ["application_date"] = request.ApplicationDate,
    //            ["recruiter_name"] = request.RecruiterNameForContext,
    //            ["recruiter_email"] = request.RecruiterEmailForContext
    //        };

    //        var subject = PopulateTemplate(subjectTemplate, values);
    //        var body = PopulateTemplate(bodyTemplate, values);

    //        await _emailService.SendEmailAsync(request.ToEmail, subject, body, isHtml: true);
    //        return Ok("Onboarding email sent.");
    //    }

    #endregion
}
