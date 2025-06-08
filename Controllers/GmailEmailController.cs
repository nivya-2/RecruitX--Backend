using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OutlookEmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public OutlookEmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendTestEmail([FromQuery] string to)
    {
        // Subject clearly states this is a test email sent via Microsoft Graph API
        var subject = "Notification: Test Email Sent via Microsoft Graph API";

        // Body explains the context, purpose, and origin of the email
        var body = @"Hello,

This is a test email sent using the Microsoft Graph API with application permissions.
It confirms that the email sending functionality is working correctly from our backend service.

If you received this email, the integration with Microsoft Graph API for sending emails is successful.

Best regards,
Your Application Team";

        await _emailService.SendEmailAsync(to, subject, body);
        return Ok("Email sent successfully.");
    }
}
