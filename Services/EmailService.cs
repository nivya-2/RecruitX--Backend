using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public interface IGmailEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
}

public record SmtpSettings
{
    public string Host { get; init; } = "";
    public int Port { get; init; }
    public string From { get; init; } = "";
    public string AppPassword { get; init; } = "";
}

public class GmailEmailService : IGmailEmailService
{
    private readonly SmtpSettings _settings;

    public GmailEmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.From, _settings.AppPassword),
            EnableSsl = true
        };

        using var mail = new MailMessage
        {
            From = new MailAddress(_settings.From),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        mail.To.Add(to);

        await smtp.SendMailAsync(mail);
    }
}
