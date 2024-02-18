using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LOVIT.Tracker.Services;

public class SlackService
{
    private readonly ILogger<SlackService> _logger;
    private const string CHANNEL_EXCEPTIONS = "https://hooks.slack.com/services/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";
    private const string CHANNEL_CHECKINS = "https://hooks.slack.com/services/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";
    private const string CHANNEL_MESSAGES = "https://hooks.slack.com/services/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";
    private const string CHANNEL_USERS = "https://hooks.slack.com/services/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";
    private const string CHANNEL_MONITORS = "https://hooks.slack.com/services/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";
    private const string CHANNEL_ACTIONS = "https://hooks.slack.com/services/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";

    public SlackService(ILogger<SlackService> logger)
    {
        _logger = logger;
    }

    public async Task PostMessageAsync(string text, Channel channel)
    {
        var uri = GetChannelUri(channel);
        /*
        using (var httpClient = new HttpClient())
        {
            try
            {
                var contentObject = new { text = text };
                var contentObjectJson = JsonSerializer.Serialize(contentObject);
                var content = new StringContent(contentObjectJson, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Start SlackService.PostMessage: {uri}");
                await httpClient.PostAsync(uri, content);
                _logger.LogInformation(string.Format($"End SlackService.PostMessage: {uri}"));
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout)
                {
                    _logger.LogInformation(string.Format($"Exception SlackService.PostMessage: {uri} \n {ex.ToString()}"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format($"Exception SlackService.PostMessage: {uri} \n {ex.ToString()}"));
                throw;
            }
        }
        */
    }

    private Uri GetChannelUri(Channel channel)
    {
        switch (channel)
        {
            case Channel.Checkins:
                return new Uri(CHANNEL_CHECKINS);
            case Channel.Exceptions:
                return new Uri(CHANNEL_EXCEPTIONS);
            case Channel.Messages:
                return new Uri(CHANNEL_MESSAGES);
            case Channel.Users:
                return new Uri(CHANNEL_USERS);
            case Channel.Monitors:
                return new Uri(CHANNEL_MONITORS);
            case Channel.Actions:
                return new Uri(CHANNEL_ACTIONS);
            default:
                return new Uri(CHANNEL_EXCEPTIONS);
        }
    }

    public enum Channel {
        Checkins,
        Exceptions,
        Messages,
        Users,
        Monitors,
        Actions
    }
}