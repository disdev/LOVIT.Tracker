using System;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Utilities;
using NuGet.Common;

namespace LOVIT.Tracker.Services;

public interface ICheckinService
{
    Task<List<Checkin>> GetCheckinsAsync();
    Task<List<Checkin>> GetCheckinsAsync(int count);
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
    Task<Checkin> ModifyCheckinAsync(Checkin checkin);
}

public class CheckinService : ICheckinService
{
    private readonly TrackerContext _context;
    private readonly IParticipantService _participantService;
    private readonly IMonitorService _monitorService;
    private readonly IWatcherService _watcherService;
    private readonly ISegmentService _segmentService;
    private readonly ITextService _textService;
    private readonly ILeaderService _leaderService;
    private readonly IPredictionService _predictionService;

    public CheckinService(TrackerContext context, IParticipantService participantService, IMonitorService monitorService, IWatcherService watcherService, ISegmentService segmentService, ILeaderService leaderService, ITextService textService, IPredictionService predictionService)
    {
        _context = context;
        _participantService = participantService;
        _monitorService = monitorService;
        _watcherService = watcherService;
        _segmentService = segmentService;
        _textService = textService;
        _leaderService = leaderService;
        _predictionService = predictionService;
    }

    public async Task<List<Checkin>> GetCheckinsAsync()
    {
        return await _context.Checkins
            .OrderBy(x => x.When)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Checkin>> GetCheckinsAsync(int count)
    {
        return await _context.Checkins
            .OrderByDescending(x => x.When)
            .Take(count)
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
                SegmentPredictionModelInput prediction = new SegmentPredictionModelInput();

                // If so, create the checkin. Automatically confirm it if it's the next segment in order.
                checkin = await InsertCheckinAsync(participant.Id, segment, checkinTime, confirmAutomatically, message.Id, segmentTime);
                checkinSegmentOrder = segment.Order;

                // Send a message to Slack
                var slackMessage = $"{participant.FullName} ({participant.Bib}) checked into {segment.ToCheckpoint?.Name}, {segment.TotalDistance} miles. ";
                slackMessage += $"{segment.Distance} at {segmentPaceString} pace";
                
                if (confirmAutomatically)
                {
                    // Get the prediction for the next segment
                    var finished = (segments.Last().Order == checkinSegmentOrder) ? true : false;
                    uint predictedElapsed = 0;
                    
                    if (finished)
                    {
                        await _participantService.SetParticipantStatusAsync(participant.Id, Status.Finished);
                    }
                    else
                    {
                        // Get the prediction for their arrival
                        prediction = await _predictionService.GetEstimateAsync(participant, segment, race.Code, overallTime);
                        predictedElapsed = (uint)prediction.SegmentElapsed;
                        var predictedDateTime = checkinTime.AddSeconds(prediction.SegmentElapsed);

                        // if confirmed, and it's the first notification for a segment, send a notification to the monitors
                        //await NotifyMonitorIfFirst(segments.Skip(skipIndex).First(), predictedDateTime);
                    }

                    // Update leader and notify watchers
                    var leader = await _leaderService.UpdateLeaderAsync(participant.Id, segment.ToCheckpointId.Value, segment.Id, checkin.Id, overallTime, Convert.ToUInt32(overallPaceInSeconds), predictedElapsed);
                    await _watcherService.NotifyWatchersAsync(participant, segment, checkin);
                }
                else
                {
                    // If the checkin isn't automatically confirmed, send a message to the admin.
                    await _textService.SendAdminMessageAsync($"Checkin to confirm for {participant.FullName}.");
                }

                break;
            }
        }

        if (checkinSegmentOrder == -1 && checkin.Id == Guid.Empty)
        {
            await _textService.SendAdminMessageAsync($"Error checking in #{bib} - {participant.FullName}. No monitor recognized.");
            return 0;
        }

        return 1;
    }

    private async Task NotifyMonitorIfFirst(Segment segment, DateTime prediction)
    {
        var count = await _context.Checkins.CountAsync(x => x.SegmentId == segment.Id);
        if (count == 1) {
            var nextSegment = await _segmentService.GetNextSegment(segment.Id);
            var checkpointMonitors = await _monitorService.GetMonitorsForCheckpointAsync(nextSegment.ToCheckpointId.Value);
            var message = $"The first participant has checked into {nextSegment.FromCheckpoint.Name} and is headed to {nextSegment.ToCheckpoint.Name}. Estimated arrival at {prediction.ToLocalTime().ToShortTimeString().ToString()}";

            foreach (var checkpointMonitor in checkpointMonitors)
            {
                await _textService.SendMessageAsync(checkpointMonitor.PhoneNumber, message);
            }
            await _textService.SendAdminMessageAsync(message);
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
        var segment = await _segmentService.GetSegmentAsync(segmentId.Value);
        var overallTime = Convert.ToUInt32((checkin.When - participant.Race.Start).TotalSeconds);
        var overallPaceInSeconds = TimeHelpers.CalculatePaceInSeconds(overallTime, segment.TotalDistance);
        var finishSegment = await _segmentService.GetFinishSegment(participant.RaceId);
        var lastCheckinTime = checkins.Count > 0 ? checkins.OrderByDescending(x => x.When).First().When : participant.Race!.Start;
        // Get the prediction for their arrival
        var prediction = await _predictionService.GetEstimateAsync(participant, segment, participant.Race.Code, overallTime);
        var predictedDateTime = checkin.When.AddSeconds(prediction.SegmentElapsed);

        checkin.Confirmed = true;

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
            //await NotifyMonitorIfFirst(segment, DateTime.Now);
        }
        
        await _context.SaveChangesAsync();
        var leader = await _leaderService.UpdateLeaderAsync(participant.Id, segment.ToCheckpointId.Value, segment.Id, checkin.Id, overallTime, Convert.ToUInt32(overallPaceInSeconds), (uint)prediction.SegmentElapsed);
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

    public async Task<Checkin> ModifyCheckinAsync(Checkin checkinValues)
    {
        var checkins = await GetCheckinsForParticipantAsync(checkinValues.ParticipantId.GetValueOrDefault());
        
        if (checkins.Count > 2)
        {
            var priorCheckin = checkins[checkins.Count - 2];
            var segmentElapsed = (checkinValues.When - priorCheckin.When).TotalSeconds;
            checkinValues.Elapsed = (uint)segmentElapsed;
        }
        
        var checkin = checkins.Where(ci => ci.Id == checkinValues.Id).First();
        checkin.When = checkinValues.When;
        checkin.ParticipantId = checkinValues.ParticipantId;
        checkin.SegmentId = checkinValues.SegmentId;
        checkin.Confirmed = checkinValues.Confirmed;
        checkin.Note = checkinValues.Note; 
        checkin.MessageId = checkinValues.MessageId;
        checkin.Elapsed = checkinValues.Elapsed;

        await _context.SaveChangesAsync();
        await _leaderService.FixLeaderAsync(checkin.ParticipantId.GetValueOrDefault());

        return checkin;
    }
}