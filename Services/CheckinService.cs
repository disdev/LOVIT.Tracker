using System;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Utilities;

namespace LOVIT.Tracker.Services;

public interface ICheckinService
{
    Task<List<Checkin>> GetCheckinsAsync();
    Task<List<Checkin>> GetCheckinsAsync(Guid raceId);
    Task<List<Checkin>> GetUnconfirmedCheckinsAsync();
    Task<Checkin> GetCheckinAsync(Guid checkinId);
    Task<Checkin?> GetCheckinAsync(Guid participantId, int order);
    Task<List<Checkin>> GetCheckinsForParticipantAsync(Guid participantId);
    Task<List<Checkin>> GetCheckinsForSegmentAsync(Guid segmentId);
    Task<Int16> HandleCheckinsAsync(Message message);
    Task<Int16> HandleCheckinAsync(string bib, Message message);
    Task<Checkin> ConfirmCheckinAsync(Guid checkinId, DateTime? when = null, Guid? segmentId = null);
    Task<Checkin> GetLastCheckinForParticipant(Guid participantId);
    Task<List<Checkin>> GetCheckinsForCheckpointAsync(Guid checkpointId);
}

public class CheckinService : ICheckinService
{
    private readonly TrackerContext _context;
    private readonly IParticipantService _participantService;
    private readonly IMonitorService _monitorService;
    private readonly IWatcherService _watcherService;
    private readonly ISegmentService _segmentService;
    private readonly ITwilioService _twilioService;
    //private readonly ILeaderboardService _leaderboardService;
    private readonly SlackService _slackService;
    private readonly ILeaderService _leaderService;

    public CheckinService(TrackerContext context, IParticipantService participantService, IMonitorService monitorService, IWatcherService watcherService, ISegmentService segmentService, SlackService slackService, ILeaderService leaderService, ITwilioService twilioService)
    {
        _context = context;
        _participantService = participantService;
        _monitorService = monitorService;
        _watcherService = watcherService;
        _segmentService = segmentService;
        _twilioService = twilioService;
        _slackService = slackService;
        _leaderService = leaderService;
    }

    public async Task<List<Checkin>> GetCheckinsAsync()
    {
        return await _context.Checkins
            .OrderBy(x => x.When)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Checkin>> GetCheckinsAsync(Guid raceId)
    {
        return await _context.Checkins
            .Where(x => x.Participant.RaceId == raceId && x.Confirmed == true)
            .Include(x => x.Segment)
            .OrderBy(x => x.When)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Checkin>> GetUnconfirmedCheckinsAsync()
    {
        return await _context.Checkins
            .Where(x => x.Confirmed == false)
            .Include(x => x.Participant)
            .Include(x=> x.Segment)
            .Include(x => x.Message)
            .OrderBy(x => x.When)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Checkin> GetCheckinAsync(Guid checkinId)
    {
        return await _context.Checkins.Where(x => x.Id == checkinId).FirstAsync();
    }

    public async Task<Checkin?> GetCheckinAsync(Guid participantId, int order)
    {
        return await _context.Checkins.FirstOrDefaultAsync(ci => ci.ParticipantId == participantId && ci.Segment.Order == order);
    }

    public async Task<List<Checkin>> GetCheckinsForParticipantAsync(Guid participantId)
    {
        return await _context.Checkins
            .Where(x => x.ParticipantId == participantId && x.Confirmed == true)
            .OrderBy(x => x.Segment.Order)
            .Include(x => x.Segment)
            .ToListAsync();
    }

    public async Task<List<Checkin>> GetCheckinsForSegmentAsync(Guid segmentId)
    {
        return await _context.Checkins
            .Where(ci => ci.SegmentId == segmentId && ci.Confirmed == true)
            .Include(x => x.Participant)
            .Include(x => x.Message)
            .Include(x => x.Segment)
            .OrderBy(x => x.When)
            .ToListAsync();
    }

    public async Task<Int16> HandleCheckinsAsync(Message message)
    {
        var messageParts = message.Body.Trim().Split(' ');
        Int16 checkinCount = 0;

        foreach (var part in messageParts)
        {
            checkinCount += await HandleCheckinAsync(part, message);
        }

        return checkinCount;
    }

    public async Task<Int16> HandleCheckinAsync(string bib, Message message)
    {
        var participant = await _participantService.GetParticipantAsync(bib, true);
        var checkins = await GetCheckinsForParticipantAsync(participant.Id);
        var monitors = await _monitorService.GetMonitorsForPhoneNumberAsync(message.From);
        var race = participant.Race;
        var segments = await _segmentService.GetSegmentsAsync(participant.RaceId);
        var checkin = new Checkin() { Id = Guid.Empty };
        int checkinSegmentOrder = -1;
        var checkinTime = message.Received;

        if (participant.Status != Status.Started)
        {
            throw new InvalidDataException("Participant has not started the race but a checkin is being recorded!");
        }

        DateTime lastCheckinTime;
        int skipIndex;

        if (checkins.Count == 0)
        {
            lastCheckinTime = participant.Race!.Start;
            skipIndex = 0;
        }
        else
        {
            lastCheckinTime = checkins.Last().When;
            skipIndex = segments.FindIndex(x => x.Id == checkins.Last().SegmentId) + 1;
        }
        
        foreach (var segment in segments.Skip(skipIndex))
        {
            // Do the monitor records for this phone number match one of the upcoming segments in the race for this participant?
            if (monitors.Any(x => x.CheckpointId == segment.ToCheckpointId))
            {
                var overallTime = Convert.ToUInt32((checkinTime - race.Start).TotalSeconds);
                var segmentTime = Convert.ToUInt32((checkinTime - lastCheckinTime).TotalSeconds);
                var overallPaceInSeconds = TimeHelpers.CalculatePaceInSeconds(overallTime, segment.TotalDistance);
                var overallPaceString = TimeHelpers.CalculatePace(overallTime, segment.TotalDistance);
                var segmentPaceString = TimeHelpers.CalculatePace(segmentTime, segment.Distance);
                var confirmAutomatically = segment.Order == skipIndex + 1;

                // If so, create the checkin. Automatically confirm it if it's the next segment in order.
                checkin = await InsertCheckinAsync(participant.Id, segment, checkinTime, confirmAutomatically, message.Id, segmentTime);
                checkinSegmentOrder = segment.Order;

                // Send a message to Slack
                var slackMessage = $"{participant.FullName} ({participant.Bib}) checked into {segment.ToCheckpoint?.Name}, {segment.TotalDistance} miles. ";
                slackMessage += $"{segment.Distance} at {segmentPaceString} pace";
                
                if (confirmAutomatically)
                {
                    // Update leader if confirmed
                    var leader = await _leaderService.UpdateLeaderAsync(participant.Id, segment.ToCheckpointId.Value, segment.Id, checkin.Id, overallTime, Convert.ToUInt32(overallPaceInSeconds));

                    // If the checkin is confirmed, send the watcher notifications.
                    await _watcherService.NotifyWatchersAsync(participant, segment, checkin);

                    // If this is the last segment, mark them as finished.
                    if (segments.Last().Order == checkinSegmentOrder)
                    {
                        await _participantService.SetParticipantStatusAsync(participant.Id, Status.Finished);
                    }
                    
                    // if confirmed, and it's the first notification for a segment, send a notification to the monitors
                    await NotifyMonitorIfFirst(segments.Skip(skipIndex).First());
                }
                else
                {
                    // If the checkin isn't automatically confirmed, send a message to the admin.
                    slackMessage = $"CHECKIN TO CONFIRM: {slackMessage}";
                    await _twilioService.SendAdminMessageAsync($"Checkin to confirm for {participant.FullName}.");
                }

                // Post the slack message.
                await _slackService.PostMessageAsync(slackMessage, SlackService.Channel.Checkins);
                
                break;
            }
        }
        // TODO: What to do if no monitor found?

        if (checkinSegmentOrder == -1 && checkin.Id == Guid.Empty)
        {
            await _twilioService.SendAdminMessageAsync($"Error checking in #{bib} - {participant.FullName}. No monitor recognized.");
            return 0;
        }

        return 1;
    }

    private async Task NotifyMonitorIfFirst(Segment segment)
    {
        var count = await _context.Checkins.CountAsync(x => x.SegmentId == segment.Id);
        if (count == 1) {
            var nextSegment = await _segmentService.GetNextSegment(segment.Id);
            var checkpointMonitors = await _monitorService.GetMonitorsForCheckpointAsync(nextSegment.ToCheckpointId.Value);

            foreach (var checkpointMonitor in checkpointMonitors)
            {
                await _twilioService.SendMessageAsync(checkpointMonitor.PhoneNumber, $"The first participant has checked into {nextSegment.FromCheckpoint.Name} and is headed to {nextSegment.ToCheckpoint.Name}.");
            }
            await _twilioService.SendAdminMessageAsync($"The first participant has checked into {segment.FromCheckpoint.Name} and is headed to {segment.ToCheckpoint.Name}.");
        }
    }

    private async Task<Checkin> InsertCheckinAsync(Guid participantId, Segment segment, DateTime when, bool confirmed, Guid messageId, uint segmentElapsed)
    {
        var checkin = new Checkin()
        {
            Id = Guid.NewGuid(),
            ParticipantId = participantId,
            SegmentId = segment.Id,
            When = when,
            Confirmed = confirmed,
            MessageId = messageId,
            Elapsed = segmentElapsed
        };

        _context.Checkins.Add(checkin);
        await _context.SaveChangesAsync();

        if (confirmed)
        {
            //await _participantService.UpdateParticipantWithCheckinAsync(checkin, segment);
        }

        return checkin;
    }

    public async Task<Checkin> ConfirmCheckinAsync(Guid checkinId, DateTime? when = null, Guid? segmentId = null)
    {
        var checkin = await _context.Checkins.Where(ci => ci.Id == checkinId).FirstAsync();
        var checkins = await GetCheckinsForParticipantAsync(checkin.ParticipantId.GetValueOrDefault());
        var participant = await _participantService.GetParticipantAsync(checkin.ParticipantId.GetValueOrDefault(), true);
        var segment = await _segmentService.GetSegmentAsync(checkin.SegmentId.GetValueOrDefault());
        var overallTime = Convert.ToUInt32((checkin.When - participant.Race.Start).TotalSeconds);
        var overallPaceInSeconds = TimeHelpers.CalculatePaceInSeconds(overallTime, segment.TotalDistance);
        var finishSegment = await _segmentService.GetFinishSegment(participant.RaceId);
        var lastCheckinTime = checkins.Count > 0 ? checkins.OrderByDescending(x => x.When).First().When : participant.Race!.Start;

        checkin.Confirmed = true;

        var leader = await _leaderService.UpdateLeaderAsync(participant.Id, segment.ToCheckpointId.Value, segment.Id, checkin.Id, overallTime, Convert.ToUInt32(overallPaceInSeconds));

        if (segmentId.HasValue)
        {
            checkin.SegmentId = segmentId.Value;
        }

        if (when.HasValue)
        {
            checkin.Elapsed = (uint)(checkin.When - lastCheckinTime).TotalSeconds;
            checkin.When = when.Value;
        }

        if (segment.Id == finishSegment.Id)
        {
            await _participantService.SetParticipantStatusAsync(participant.Id, Status.Finished);
        }
        else
        {
            await NotifyMonitorIfFirst(segment);
        }
        
        await _context.SaveChangesAsync();

        //await _participantService.UpdateParticipantWithCheckinAsync(checkin, segment);
        await _watcherService.NotifyWatchersAsync(participant, segment, checkin);

        return checkin;
    }

    public async Task<Checkin> GetLastCheckinForParticipant(Guid participantId)
    {
        return await _context.Checkins
            .Where(x => x.ParticipantId == participantId)
            .OrderByDescending(x => x.Segment.Order)
            .Include(x => x.Segment)
            .FirstAsync();
    }

    public async Task<List<Checkin>> GetCheckinsForCheckpointAsync(Guid checkpointId)
    {
        return await _context.Checkins
            .Where(ci => ci.Segment.ToCheckpointId == checkpointId)
            .Where(x => x.Confirmed == true)
            .Include(x => x.Participant)
            .Include(x => x.Message)
            .Include(x => x.Segment)
            .AsNoTracking()
            .ToListAsync();
    }
}