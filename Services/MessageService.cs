using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using System.Text.Json;
using Auth0.ManagementApi.Models;
using Newtonsoft.Json.Linq;
namespace LOVIT.Tracker.Services;

public interface IMessageService
{
    Task<List<Message>> GetMessagesAsync();
    Task<Message> GetMessageAsync(Guid messageId);
    Task<Message> AddMessageAsync(Message message);
    Task<Message> AddMessageAsync(HttpContext httpContext);
    Task<string> HandleMessageAsync(Message message);
}

public class MessageService : IMessageService
{
    private readonly TrackerContext _context;
    private readonly IRaceService _raceService;
    private readonly IMonitorService _monitorService;
    private readonly IWatcherService _watcherService;
    private readonly ITextService _textService;
    private readonly ICheckinService _checkinService;
    private readonly IParticipantService _participantService;

    public MessageService(TrackerContext context, IRaceService raceService, IMonitorService monitorService, IWatcherService watcherService, ICheckinService checkinService, ITextService textService, IParticipantService participantService)
    {
        _context = context;
        _raceService = raceService;
        _monitorService = monitorService;
        _watcherService = watcherService;
        _textService = textService;
        _checkinService = checkinService;
        _participantService = participantService;
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
        // THIS IS ONLY USED BY THE SEED SERVICE
        message.Id = Guid.NewGuid();
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<Message> AddMessageAsync(HttpContext httpContext)
    {
        // THIS IS USED BY THE MESSAGE HANDLER
        var responseString = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
        JObject responseJson = JObject.Parse(responseString);
        //var formData = httpContext.Request.Form;
        var message = new Message()
        {
            From = responseJson["fromNumber"]!.ToString(),
            FromCity = "",
            FromState = "",
            FromCountry = "",
            FromZip = "",
            Body = responseJson["text"]!.ToString(),
            Received = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<string> HandleMessageAsync(Message message)
    {
        var adminPhone = _textService.GetAdminPhone();
        // First check that we know who this is.
        var isValidMonitor = await _monitorService.IsValidMonitor(message.From);
        if (!isValidMonitor && !message.Body.ToUpper().StartsWith("SETUP") && message.From != adminPhone)
        {
            await _textService.SendAdminMessageAsync($"Bad message from {message.From.ToString()}. Monitor: {isValidMonitor.ToString()}. Message: {message.Body}.");
            return $"This is an automated system that handles race updates. We cannot respond to incoming messages.";
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
            case "DROP":
                return await HandleDropMessage(messageIntent, message);
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
            return new ("CHECKIN", messageParts.ToArray());
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

        if (messageParts[0].Trim().ToUpper(CultureInfo.InvariantCulture) == "DROP")
        {
            return new ("DROP", messageParts.Skip(1).ToArray());
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
        var race = await _raceService.StartRace(message.From, messageIntent.Item2[0].ToUpper(), message.Received);
        return $"Started {race.Code}.";
    }

    private async Task<string> HandleSetupMessage(Tuple<string, string[]> messageIntent, Message message)
    {
        var monitor = await _monitorService.AddMonitor(message.From, Convert.ToInt16(messageIntent.Item2[0], CultureInfo.InvariantCulture));
        await _textService.SendAdminMessageAsync($"{message.From} is a monitor for {monitor.Checkpoint?.Name}");
        return $"You're set up as a monitor for {monitor.Checkpoint?.Name}.";
    }

    private async Task<string> HandleDropMessage(Tuple<string, string[]> messageIntent, Message message)
    {
        if (message.From == _textService.GetAdminPhone())
        {
            foreach (var bib in messageIntent.Item2)
            {
                var participant = await _participantService.GetParticipantAsync(bib);
                await _participantService.DropParticipantAsync(bib);
                await _textService.SendAdminMessageAsync($"{participant.FullName} ({bib}) had been dropped from the race.");
            }
        }

        return "Completed processing drops.";
    }

    private async Task<string> HandleStopMessage(Tuple<string, string[]> messageIntent, Message message)
    {
        await _watcherService.DisableAllWatchersForPhoneAsync(message.From);
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
        return "I only understand race bib numbers. Your message has been forwarded to the race director for review.";
    }
}