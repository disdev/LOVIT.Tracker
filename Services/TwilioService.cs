using System;
using LOVIT.Tracker.Models;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Lookups.V1;
using Twilio.Rest.Notify.V1.Service;
using NotificationResource = Twilio.Rest.Notify.V1.Service.NotificationResource;
using Microsoft.Extensions.Logging;
using LOVIT.Tracker.Data;

namespace LOVIT.Tracker.Services;

public interface ITwilioService
{
    Task SendMessageAsync(string toNumber, string body);
    Task SendAdminMessageAsync(string body);
    Task<string> CheckPhoneNumberAsync(string inputNumber);
    Task CreateBindingAsync(string userId, string phoneNumber);
    Task SendMessageAsync(Watcher watcher, string body);
    Task SendMessageAsync(List<Watcher> watchers, string body);
}

public class TwilioService : ITwilioService
{
    private TwilioSettings _twilioSettings;
    private readonly SlackService _slackService;
    private readonly ILogger<TwilioService> _logger;

    public TwilioService(TrackerContext context, IOptions<TwilioSettings> twilioSettings, SlackService slackService, ILogger<TwilioService> logger)
    {
        _twilioSettings = twilioSettings.Value;
        _slackService = slackService;
        _logger = logger;

        TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthKey);
    }

    public async Task SendMessageAsync(string toNumber, string body)
    {
        if (_twilioSettings.Enabled)
        {
            try
            {
                await MessageResource.CreateAsync(
                    new PhoneNumber(toNumber),
                    from: new PhoneNumber(_twilioSettings.SystemPhone),
                    body: body);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error in TwilioService: {ex.ToString()}");
            }
        }
    }

    public async Task SendMessageAsync(Watcher watcher, string body)
    {
        if (_twilioSettings.Enabled)
        {
            try
            {
                var notification = await NotificationResource.CreateAsync(
                    _twilioSettings.NotificationSid,
                    identity: new List<string> { watcher.UserId },
                    body: body);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error in TwilioService: {ex.ToString()}");
                await _slackService.PostMessageAsync($"Error in TwilioService: {ex.ToString()}", SlackService.Channel.Exceptions);
            }
        }
    }

    public async Task SendMessageAsync(List<Watcher> watchers, string body)
    {
        if (_twilioSettings.Enabled)
        {
            var phoneNumbers = watchers.Select(x => x.UserId).ToList();

            try
            {
                var notification = await NotificationResource.CreateAsync(
                    _twilioSettings.NotificationSid,
                    identity: phoneNumbers,
                    body: body);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error in TwilioService: {ex.ToString()}");
                await _slackService.PostMessageAsync($"Error in TwilioService: {ex.ToString()}", SlackService.Channel.Exceptions);
            }
        }
    }

    public async Task CreateBindingAsync(string userId, string phoneNumber)
    {
        var binding = await BindingResource.CreateAsync(
            identity: userId,
            bindingType: BindingResource.BindingTypeEnum.Sms,
            address: phoneNumber,
            pathServiceSid: _twilioSettings.NotificationSid
        );
    }

    public async Task SendAdminMessageAsync(string body)
    {
        if (_twilioSettings.Enabled)
        {
            try
            {
                await SendMessageAsync(_twilioSettings.AdminPhone, body);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error in TwilioService: {ex.ToString()}");
                await _slackService.PostMessageAsync($"Error in TwilioService: {ex.ToString()}", SlackService.Channel.Exceptions);
            }
        }
    }

    public async Task<string> CheckPhoneNumberAsync(string inputNumber)
    {
        if (_twilioSettings.Enabled)
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
}