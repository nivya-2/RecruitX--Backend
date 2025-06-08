using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GraphEmailService : IEmailService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IConfiguration _config;
    private readonly ILogger<GraphEmailService> _logger;

    public GraphEmailService(
        GraphServiceClient graphClient,
        IConfiguration config,
        ILogger<GraphEmailService> logger)
    {
        _graphClient = graphClient;
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var fromAddress = _config["EmailSettings:SenderAddress"];

        var message = new Message
        {
            Subject = subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = htmlBody
            },
            ToRecipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress { Address = toEmail }
                }
            }
        };

        var sendMailBody = new SendMailPostRequestBody
        {
            Message = message,
            SaveToSentItems = true
        };

        try
        {
            await _graphClient.Users[fromAddress].SendMail.PostAsync(sendMailBody);
            _logger.LogInformation("Email sent successfully to {ToEmail} from {FromAddress} at {Time}", toEmail, fromAddress, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail} from {FromAddress}", toEmail, fromAddress);
            throw; // rethrow so caller knows about failure
        }
    }
}
