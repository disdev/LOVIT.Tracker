using System;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Services;

public interface IWatcherService
{
    Task<List<Watcher>> GetWatchersAsync();
    Task<List<Watcher>> GetWatchersForParticipantAsync(Guid participantId);
    Task<Watcher> GetWatcherAsync(Guid watcherId);
    Task DisableAllWatchersForPhoneAsync(string phoneNumber);
    Task NotifyWatchersAsync(Participant participant, Segment segment, Checkin checkin);
    Task<Watcher> AddWatcherAsync(Guid participantId, string userId, string phoneNumber);
    Task UpdateUserPhoneNumberAsync(string UserId, string phoneNumber);
    Task RemoveWatcher(Guid participantId, string userId);
    Task<bool> WatcherExists(Guid participantId, string userId);
}

public class WatcherService : IWatcherService
{
    private readonly TrackerContext _context;
    private readonly SlackService _slackService;
    private readonly IParticipantService _participantService;
    private readonly ITextService _TextService;

    public WatcherService(TrackerContext context, SlackService slackService, IParticipantService participantService, ITextService TextService)
    {
        _context = context;
        _slackService = slackService;
        _participantService = participantService;
        _TextService = TextService;
    }

    public async Task<List<Watcher>> GetWatchersAsync()
    {
        return await _context.Watchers.Where(x => x.Disabled == false).Include(x => x.Participant).ToListAsync();
    }
    
    public async Task<List<Watcher>> GetWatchersForParticipantAsync(Guid participantId)
    {
        return await _context.Watchers.Where(x => x.ParticipantId == participantId && x.Disabled == false).ToListAsync();
    }

    public async Task<Watcher> GetWatcherAsync(Guid watcherId)
    {
        return await _context.Watchers.Where(x => x.Id == watcherId).FirstAsync();
    }

    public async Task DisableAllWatchersForPhoneAsync(string phoneNumber)
    {
        var watchers = await _context.Watchers.Where(x => x.PhoneNumber == phoneNumber).ToListAsync();

        foreach (var watcher in watchers)
        {
            watcher.Disabled = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task NotifyWatchersAsync(Participant participant, Segment segment, Checkin checkin)
    {
        var watchers = await _context.Watchers.Where(x => x.ParticipantId == participant.Id && x.Disabled == false).ToListAsync();

        foreach (var watcher in watchers)
        {
            await _TextService.SendMessageAsync(watcher.PhoneNumber, $"{participant.FullName} checked into {segment.ToCheckpoint.Name} at {segment.TotalDistance} miles at {checkin.When.ToLocalTime().ToShortTimeString()}. https://track.runlovit.com/participants?id={participant.Bib}");
            // await _TextService.SendMessageAsync(watcher, $"{participant.FullName} checked into {segment.ToCheckpoint.Name} at {segment.TotalDistance} miles.");
        }
    }

    public async Task<Watcher> AddWatcherAsync(Guid participantId, string userId, string phoneNumber)
    {
        var watcher = await _context.Watchers.FirstOrDefaultAsync(w => w.ParticipantId == participantId && w.PhoneNumber == w.PhoneNumber && w.UserId == userId);
        var participant = await _participantService.GetParticipantAsync(participantId);
        
        if (watcher == null)
        {
            watcher = new Watcher()
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber,
                ParticipantId = participantId,
                Disabled = false,
                UserId = userId
            };
            _context.Watchers.Add(watcher);
            await _context.SaveChangesAsync();
        }
        else
        {
            if (watcher.Disabled == true)
            {
                watcher.Disabled = false;
                await _context.SaveChangesAsync();
            }
        }

        // await _TextService.SendMessageAsync(watcher, $"You're set up to receive LOViT race updates for {participant.FullName}. Reply STOP to end.");
        await _slackService.PostMessageAsync($"Watcher enabled for {participant.FullName}. Phone: {phoneNumber}, User: {userId}", SlackService.Channel.Actions);
        
        return watcher;
    }

    public async Task UpdateUserPhoneNumberAsync(string UserId, string phoneNumber)
    {
        // TODO: This
        throw new NotImplementedException();
    }

    public async Task RemoveWatcher(Guid participantId, string userId)
    {
        var watcher = await _context.Watchers.SingleAsync(w => w.ParticipantId == participantId && w.UserId == userId);
        _context.Watchers.Remove(watcher);


        await _context.SaveChangesAsync();
    }

    public async Task<bool> WatcherExists(Guid participantId, string userId)
    {
        return await _context.Watchers.Where(w => w.ParticipantId == participantId && w.UserId == userId).AnyAsync();
    }
}