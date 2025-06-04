using Microsoft.AspNetCore.Mvc;
using RecruitX.Services;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendTestEmail([FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(to))
            return BadRequest("Recipient address is required.");

        await _emailService.SendEmailAsync(to, "Test Email", "<strong>Hello from your app!</strong>");
        return Ok("Email sent.");
    }
}
