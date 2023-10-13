using Azure.Identity;
using LOVIT.Tracker.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace LOVIT.Tracker.Services;

public interface IGraphMailService
{
    Task SendAsync(string fromAddress, string toAddress, string toName, string subject, string content);
}

public class GraphMailService : IGraphMailService
{
    private readonly GraphMailConfig _graphMailConfig;

    public GraphMailService(IOptionsMonitor<GraphMailConfig> optionsMonitor)
    {
        _graphMailConfig = optionsMonitor.CurrentValue;
    }

    public async Task SendAsync(string fromAddress, string toAddress, string toName, string subject, string content)
    {
        string? tenantId = _graphMailConfig.TenantId;
        string? clientId = _graphMailConfig.ClientId;
        string? clientSecret = _graphMailConfig.ClientSecret;

        ClientSecretCredential credential = new(tenantId, clientId, clientSecret);
        GraphServiceClient graphClient = new(credential);

        var requestBody = new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody()
        {
            Message = new()
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = content
                },
                ToRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = toAddress,
                            Name = toName
                        }
                    }
                }
            },
            SaveToSentItems = true
        };

        await graphClient.Me.SendMail.PostAsync(requestBody);
    }
}