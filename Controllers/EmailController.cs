using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using RecruitX.Controllers.RequestDTOs;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IGmailEmailService _emailService;

    public EmailController(IGmailEmailService emailService)
    {
        _emailService = emailService;
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

        const string subjectTemplate = "Initial Screening for {{job_title}} Position";
        const string bodyTemplate = @"<p>Dear {{candidate_name}},</p>
<p>Thank you for your interest in the {{job_title}} role at {{company_name}}.</p>
<p>We have reviewed your application and would like to schedule a quick phone screening to get to know you better and share more about the opportunity.</p>
<p>Please confirm your availability for a 15-minute call with our recruiter, {{recruiter_name}}, on {{screening_date}} at {{screening_time}}.</p>
<p>We look forward to speaking with you!</p>
<p>Best regards,<br>
{{company_name}} Recruitment Team</p>";

        var values = new Dictionary<string, string?>
        {
            ["job_title"] = request.JobTitle,
            ["candidate_name"] = request.CandidateName,
            ["company_name"] = request.CompanyName,
            ["recruiter_name"] = request.RecruiterName,
            ["screening_date"] = request.ScreeningDate,
            ["screening_time"] = request.ScreeningTime,
            ["application_date"] = request.ApplicationDate,
            ["recruiter_email"] = request.RecruiterEmail
        };

        var subject = PopulateTemplate(subjectTemplate, values);
        var body = PopulateTemplate(bodyTemplate, values);

        await _emailService.SendEmailAsync(request.ToEmail, subject, body, isHtml: true);
        return Ok("Screening email sent.");
    }

    [HttpPost("send-interview-invitation")]
    public async Task<IActionResult> SendInterviewInvitationEmail([FromBody] SendInterviewInvitationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        const string subjectTemplate = "Interview Invitation for {{job_title}} Position";
        const string bodyTemplate = @"<p>Dear {{candidate_name}},</p>
<p>Thank you for applying to the {{job_title}} position at {{company_name}}. We were impressed with your profile and would like to invite you for an interview.</p>
<p>Interview Details:<br>
- Date: {{interview_date}}<br>
- Time: {{interview_time}}<br>
- Location: {{interview_location}}<br>
- Interviewer(s): {{interviewer_names}}</p>
<p>Please confirm your availability by replying to this email or contacting {{recruiter_name}} at {{recruiter_email}}.</p>
<p>Looking forward to meeting you!</p>
<p>Regards,<br>
{{company_name}} Recruitment Team</p>";

        var values = new Dictionary<string, string?>
        {
            ["job_title"] = request.JobTitle,
            ["candidate_name"] = request.CandidateName,
            ["company_name"] = request.CompanyName,
            ["interview_date"] = request.InterviewDate,
            ["interview_time"] = request.InterviewTime,
            ["interview_location"] = request.InterviewLocation,
            ["interviewer_names"] = request.InterviewerNames,
            ["recruiter_name"] = request.RecruiterName,
            ["recruiter_email"] = request.RecruiterEmail,
            ["application_date"] = request.ApplicationDate
        };

        var subject = PopulateTemplate(subjectTemplate, values);
        var body = PopulateTemplate(bodyTemplate, values);

        await _emailService.SendEmailAsync(request.ToEmail, subject, body, isHtml: true);
        return Ok("Interview invitation email sent.");
    }

    [HttpPost("send-job-offer")]
    public async Task<IActionResult> SendJobOfferEmail([FromBody] SendJobOfferRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        const string subjectTemplate = "Job Offer: {{job_title}} at {{company_name}}";
        const string bodyTemplate = @"<p>Dear {{candidate_name}},</p>
<p>We are excited to offer you the position of {{job_title}} at {{company_name}}!</p>
<ul>
    <li>Salary: {{salary}}</li>
    <li>Start Date: {{start_date}}</li>
    <li>Benefits: {{benefits_package}}</li>
    <li>Reporting Manager: {{manager_name}}</li>
</ul>
<p>To accept, please sign and return the offer letter by {{response_deadline}}.</p>
<p>If you have any questions, contact {{hr_contact}} at {{hr_email}}.</p>
<p>We hope you'll join our team!</p>
<p>Warm regards,</p>";

        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = request.CandidateName,
            ["job_title"] = request.JobTitle,
            ["company_name"] = request.CompanyName,
            ["salary"] = request.Salary,
            ["start_date"] = request.StartDate,
            ["benefits_package"] = request.BenefitsPackage,
            ["manager_name"] = request.ManagerName,
            ["response_deadline"] = request.ResponseDeadline,
            ["hr_contact"] = request.HrContact,
            ["hr_email"] = request.HrEmail,
            ["application_date"] = request.ApplicationDate,
            ["recruiter_name"] = request.RecruiterName,
            ["recruiter_email"] = request.RecruiterEmail
        };

        var subject = PopulateTemplate(subjectTemplate, values);
        var body = PopulateTemplate(bodyTemplate, values);

        await _emailService.SendEmailAsync(request.ToEmail, subject, body, isHtml: true);
        return Ok("Job offer email sent.");
    }

    [HttpPost("send-rejection")]
    public async Task<IActionResult> SendRejectionEmail([FromBody] SendRejectionEmailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        const string subjectTemplate = "Your Application for {{job_title}} at {{company_name}}";
        const string bodyTemplate = @"<p>Dear {{candidate_name}},</p>
<p>Thank you for taking the time to interview for the {{job_title}} position at {{company_name}}.</p>
<p>After careful consideration, we regret to inform you that we will not be moving forward with your application at this time.</p>
<p>We appreciate your interest in our company and encourage you to apply for future openings that match your skills and experience.</p>
<p>Wishing you all the best in your job search.</p>
<p>Sincerely,<br>
{{recruiter_name}}<br>
{{company_name}} Recruitment Team</p>";

        var values = new Dictionary<string, string?>
        {
            ["candidate_name"] = request.CandidateName,
            ["job_title"] = request.JobTitle,
            ["company_name"] = request.CompanyName,
            ["recruiter_name"] = request.RecruiterName,
            ["application_date"] = request.ApplicationDate,
            ["recruiter_email"] = request.RecruiterEmail
        };

        var subject = PopulateTemplate(subjectTemplate, values);
        var body = PopulateTemplate(bodyTemplate, values);

        await _emailService.SendEmailAsync(request.ToEmail, subject, body, isHtml: true);
        return Ok("Rejection email sent.");
    }

    [HttpPost("send-onboarding")]
    public async Task<IActionResult> SendOnboardingEmail([FromBody] SendOnboardingEmailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        const string subjectTemplate = "Welcome to {{company_name}} – Your Onboarding Details";
        const string bodyTemplate = @"<p>Dear {{employee_name}},</p>
<p>Welcome aboard!</p>
<p>We are thrilled to have you join us as a {{role}}. Your start date is {{start_date}}.</p>
<p>Please complete the following before your first day:
<ul>
    <li>Fill out the onboarding documents</li>
    <li>Set up your company email account</li>
    <li>Complete mandatory training sessions</li>
</ul>
</p>
<p>You will be reporting to {{manager_name}} and your workstation will be at {{desk_location}}.</p>
<p>For questions, contact {{hr_contact}} at {{hr_email}}.</p>
<p>We look forward to working with you!</p>";

        var values = new Dictionary<string, string?>
        {
            ["company_name"] = request.CompanyName,
            ["employee_name"] = request.EmployeeName,
            ["role"] = request.Role,
            ["start_date"] = request.StartDate,
            ["manager_name"] = request.ManagerName,
            ["desk_location"] = request.DeskLocation,
            ["hr_contact"] = request.HrContact,
            ["hr_email"] = request.HrEmail,
            ["application_date"] = request.ApplicationDate,
            ["recruiter_name"] = request.RecruiterNameForContext,
            ["recruiter_email"] = request.RecruiterEmailForContext
        };

        var subject = PopulateTemplate(subjectTemplate, values);
        var body = PopulateTemplate(bodyTemplate, values);

        await _emailService.SendEmailAsync(request.ToEmail, subject, body, isHtml: true);
        return Ok("Onboarding email sent.");
    }

    #endregion
}
