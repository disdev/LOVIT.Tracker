using System;
using LOVIT.Tracker.Models;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Lookups.V1;
using Twilio.Rest.Notify.V1.Service;
using Microsoft.Extensions.Logging;
using LOVIT.Tracker.Data;
using System.Collections.Specialized;
using System.Net;

namespace LOVIT.Tracker.Services;

public interface ITextService
{
    Task SendMessageAsync(string toNumber, string body);
    Task SendAdminMessageAsync(string body);
    Task<string> CheckPhoneNumberAsync(string inputNumber);
    Task CreateBindingAsync(string userId, string phoneNumber);
    Task SendMessageAsync(Watcher watcher, string body);
    Task SendMessageAsync(List<Watcher> watchers, string body);
}

public class TextService : ITextService
{
    private TextSettings _textSettings;
    private readonly SlackService _slackService;
    private readonly ILogger<TextService> _logger;

    public TextService(TrackerContext context, IOptions<TextSettings> textSettings, SlackService slackService, ILogger<TextService> logger)
    {
        _textSettings = textSettings.Value;
        _slackService = slackService;
        _logger = logger;

        TwilioClient.Init(_textSettings.AccountSid, _textSettings.AuthKey);
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
                await _slackService.PostMessageAsync($"Error in TextService: {ex.ToString()}", SlackService.Channel.Exceptions);
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
                await _slackService.PostMessageAsync($"Error in TextService: {ex.ToString()}", SlackService.Channel.Exceptions);
            }
        }
        else
        {
            _logger.LogInformation($"Message not sent: {body}");
        }
    }

    public async Task CreateBindingAsync(string userId, string phoneNumber)
    {
        var binding = await BindingResource.CreateAsync(
            identity: userId,
            bindingType: BindingResource.BindingTypeEnum.Sms,
            address: phoneNumber,
            pathServiceSid: _textSettings.NotificationSid
        );
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
                await _slackService.PostMessageAsync($"Error in TextService: {ex.ToString()}", SlackService.Channel.Exceptions);
            }
        }
    }

    public async Task<string> CheckPhoneNumberAsync(string inputNumber)
    {
        if (_textSettings.Enabled)
        {
            try
            {
                var phoneNumber = await PhoneNumberResource.FetchAsync(
                    pathPhoneNumber: new Twilio.Types.PhoneNumber(inputNumber)
                );

                return phoneNumber.PhoneNumber.ToString();
            }
            catch
            {
                return "";
            }
        }
        else
        {
            return inputNumber;
        }
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