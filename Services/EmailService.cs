using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RecruitX.Services;

public record SmtpSettings
{
    public string Host { get; init; } = "";
    public int Port { get; init; }
    public string From { get; init; } = "";
    public string AppPassword { get; init; } = "";
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.From, _settings.AppPassword),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_settings.From),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        await smtp.SendMailAsync(mail);
    }
}
