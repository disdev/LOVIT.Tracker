using System;
using LOVIT.Tracker.Models;
using Microsoft.Extensions.Options;
using LOVIT.Tracker.Data;

namespace LOVIT.Tracker.Services;

public interface ITextService
{
    Task SendMessageAsync(string toNumber, string body);
    Task SendAdminMessageAsync(string body);
    Task<string> CheckPhoneNumberAsync(string inputNumber);
    Task SendMessageAsync(Watcher watcher, string body);
    Task SendMessageAsync(List<Watcher> watchers, string body);
}

public class TextService : ITextService
{
    private TextSettings _textSettings;
    private readonly ILogger<TextService> _logger;

    public TextService(TrackerContext context, IOptions<TextSettings> textSettings, ILogger<TextService> logger)
    {
        _textSettings = textSettings.Value;
        _logger = logger;
    }

    public async Task SendMessageAsync(string toNumber, string body)
    {
        if (_textSettings.Enabled)
        {
            try
            {
                await SendTextMessage(toNumber, body);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error in TextService: {ex.ToString()}");
            }
        }
    }

    public async Task SendMessageAsync(Watcher watcher, string body)
    {
        if (_textSettings.Enabled)
        {
            try
            {
                await SendTextMessage(watcher.PhoneNumber, body);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error in TextService: {ex.ToString()}");
            }
        }
    }

    public async Task SendMessageAsync(List<Watcher> watchers, string body)
    {
        if (_textSettings.Enabled)
        {
            var phoneNumbers = watchers.Select(x => x.UserId).ToList();

            try
            {
                await SendTextMessages(phoneNumbers, body);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error in TextService: {ex.ToString()}");
            }
        }
        else
        {
            _logger.LogInformation($"Message not sent: {body}");
        }
    }

    public async Task SendAdminMessageAsync(string body)
    {
        if (_textSettings.Enabled)
        {
            try
            {
                await SendTextMessage(_textSettings.AdminPhone, body);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error in TextService: {ex.ToString()}");
            }
        }
    }

    public async Task<string> CheckPhoneNumberAsync(string inputNumber)
    {
        // TODO: Implement this
        return inputNumber;
    }

    private async Task<string> SendTextMessage(string number, string message)
    {
        using (HttpClient client = new HttpClient())
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("phone", number), 
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("key", _textSettings.TextBeltKey),
            });

            var response = await client.PostAsync(_textSettings.TextBeltUrl, formContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            return stringResponse;
        }
    }

    private async Task<string> SendTextMessages(List<string> numbers, string message)
    {
        using (HttpClient client = new HttpClient())
        {
            foreach (var number in numbers)
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("phone", number), 
                    new KeyValuePair<string, string>("message", message),
                    new KeyValuePair<string, string>("key", _textSettings.TextBeltKey),
                });

                var response = await client.PostAsync(_textSettings.TextBeltUrl, formContent);
            }
        }

        return "";
    }
}