using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using Twilio.AspNet.Common;

namespace LOVIT.Tracker.Services;

public interface IMessageService
{
    Task<List<Message>> GetMessagesAsync();
    Task<Message> GetMessageAsync(Guid messageId);
    Task<Message> AddMessageAsync(Message message);
    Task<Message> AddMessageAsync(SmsRequest incomingSms);
    Task<string> HandleMessageAsync(Message message);
}

public class MessageService : IMessageService
{
    private readonly TrackerContext _context;
    private readonly IRaceService _raceService;
    private readonly IMonitorService _monitorService;
    private readonly IWatcherService _watcherService;
    private readonly ITwilioService _twilioService;
    private readonly ICheckinService _checkinService;
    private readonly SlackService _slackService;

    public MessageService(TrackerContext context, IRaceService raceService, IMonitorService monitorService, IWatcherService watcherService, ICheckinService checkinService, SlackService slackService, ITwilioService twilioService)
    {
        _context = context;
        _raceService = raceService;
        _monitorService = monitorService;
        _watcherService = watcherService;
        _twilioService = twilioService;
        _checkinService = checkinService;
        _slackService = slackService;
    }

    public async Task<List<Message>> GetMessagesAsync()
    {
        return await _context.Messages.OrderBy(x => x.Received).ToListAsync();
    }

    public async Task<Message> GetMessageAsync(Guid messageId)
    {
        return await _context.Messages.Where(x => x.Id == messageId).FirstAsync();
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        message.Id = Guid.NewGuid();
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        await _slackService.PostMessageAsync($"Message: {message.Body}, From: {message.From}", SlackService.Channel.Messages);

        return message;
    }

    public async Task<Message> AddMessageAsync(SmsRequest incomingSms)
    {
        var message = new Message()
        {
            Id = Guid.NewGuid(),
            From = incomingSms.From,
            Body = incomingSms.Body.Trim(),
            FromCity = incomingSms.FromCity,
            FromState = incomingSms.FromState,
            FromZip = incomingSms.FromZip,
            FromCountry = incomingSms.FromCountry,
            Received = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        await _slackService.PostMessageAsync($"Message: {message.Body}, From: {message.From}", SlackService.Channel.Messages);

        return message;
    }

    public async Task<string> HandleMessageAsync(Message message)
    {
        // First check that we know who this is.
        var isValidMonitor = await _monitorService.IsValidMonitor(message.From);
        if (!isValidMonitor)
        {
            await _twilioService.SendAdminMessageAsync($"Bad message from {message.From.ToString()}. Monitor: {isValidMonitor.ToString()}. Message: {message.Body}.");
            await _slackService.PostMessageAsync($"{message.From} sent an unhandled message: {message.Body}", SlackService.Channel.Exceptions);
            return $"This is an automated system that handles race updates. We cannot respond personally to incoming messages.";
        }   

        // Get the message intent
        var messageIntent = GetMessageIntent(message.Body);  

        switch (messageIntent.Item1)
        {
            case "CHECKIN":
                return await HandleCheckinMessage(messageIntent, message);
            case "START":
                return await HandleStartMessage(messageIntent, message);
            case "SETUP":
                return await HandleSetupMessage(messageIntent, message);
            case "STOP":
                return await HandleStopMessage(messageIntent, message);
            case "DNS":
                return "";
            case "DNF":
                return "";
            default:
                return await HandleUnhandledMessage(messageIntent, message);
        }
    }

    private Tuple<string, string[]> GetMessageIntent(string messageBody)
    {
        var messageParts = messageBody.Trim().Split(' ');
        
        if (messageBody.Replace(" ", "").All(char.IsDigit))
        {
            return new ("CHECKIN", messageParts.Skip(1).ToArray());
        }

        if (messageParts[0].Trim().ToUpper(CultureInfo.InvariantCulture) == "START")
        {
            return new ("START", messageParts.Skip(1).ToArray());
        }

        if (messageParts[0].Trim().ToUpper(CultureInfo.InvariantCulture) == "STOP")
        {
            return new ("STOP", new string[0]);
        }

        if (messageParts[0].Trim().ToUpper(CultureInfo.InvariantCulture) == "DNS")
        {
            return new ("DNS", messageParts.Skip(1).ToArray());
        }

        if (messageParts[0].Trim().ToUpper(CultureInfo.InvariantCulture) == "DNF")
        {
            return new ("DNF", messageParts.Skip(1).ToArray());
        }

        if (messageParts[0].Trim().ToUpper(CultureInfo.InvariantCulture) == "SETUP")
        {
            return new ("SETUP", messageParts.Skip(1).ToArray());
        }

        return new ("", new string[0]);
    }

    private async Task<string> HandleStartMessage(Tuple<string, string[]> messageIntent, Message message)
    {
        var race = await _raceService.StartRace(message.From, messageIntent.Item2[0], message.Received);
        return $"Started {race.Code}.";
    }

    private async Task<string> HandleSetupMessage(Tuple<string, string[]> messageIntent, Message message)
    {
        var monitor = await _monitorService.AddMonitor(message.From, Convert.ToInt16(messageIntent.Item2[0], CultureInfo.InvariantCulture));
        await _slackService.PostMessageAsync($"{message.From} is a monitor for {monitor.Checkpoint?.Name}", SlackService.Channel.Monitors);
        return $"You're set up as a monitor for {monitor.Checkpoint?.Name}.";
    }

    private async Task<string> HandleStopMessage(Tuple<string, string[]> messageIntent, Message message)
    {
        await _watcherService.DisableAllWatchersForPhoneAsync(message.From);
        await _slackService.PostMessageAsync($"{message.From} sent a STOP message.", SlackService.Channel.Messages);
        return $"You will no longer receive race updates. If you'd like to change this, please sign up for updates online again.";
    }

    private async Task<string> HandleCheckinMessage(Tuple<string, string[]> messageIntent, Message message)
    {
        var checkinCount = await _checkinService.HandleCheckinsAsync(message);
        var responseText = $"Checked in {checkinCount} runner{(checkinCount > 1 || checkinCount == 0 ? "s" : "")}.";
        if (checkinCount == 0)
        {
            responseText += " An error has occurred. The race director has been notified.";
        }
        return responseText;
    }

    private async Task<string> HandleUnhandledMessage(Tuple<string, string[]> messageIntent, Message message)
    {
        var monitors = await _monitorService.GetMonitorsForPhoneNumberAsync(message.From);
        var monitorList = string.Join(",", monitors.Select(x => x.Checkpoint!.Name));
        await _slackService.PostMessageAsync($"Monitor {message.From} from {monitorList} sent an unhandled message: {message.Body}", SlackService.Channel.Exceptions);
        return "I only understand race bib numbers. Your message has been forwarded to the race director for review.";
    }
}