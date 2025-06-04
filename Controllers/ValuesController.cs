using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public MailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail()
    {
        await _emailService.SendEmailAsync("recipient@example.com", "Test Email", "<h1>Hello from .NET!</h1>");
        return Ok("Email sent successfully.");
    }
}
