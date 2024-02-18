using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace LOVIT.Tracker.Services;

public interface ILeaderService
{
    Task<Leader> AddLeaderAsync(Participant participant);
    Task<Leader> GetLeaderByParticipantIdAsync(Guid participantId);
    Task<Leader> UpdateLeaderAsync(Guid participantId, Guid checkpointId, Guid segmentId, Guid checkinId, uint overallTime, uint overallPace, uint nextPredictedSegmentTime);
    Task<List<Leader>> GetLeadersAsync();
    Task<List<Leader>> GetLeadersByRaceIdAsync(Guid raceId);
    Task<List<Leader>> GetLeadersByWatcherUserAsync(string userId);
    Task<List<Leader>> GetLeadersByDroppedStatusAsync();
}

public class LeaderService : ILeaderService
{
    private readonly TrackerContext _context;

    public LeaderService(TrackerContext context)
    {
        _context = context;
    }

    public async Task<Leader> AddLeaderAsync(Participant participant)
    {
        var leader = new Leader();
        leader.Id = Guid.NewGuid();
        leader.ParticipantId = participant.Id;
        leader.Participant = participant;

        _context.Leaders.Add(leader);
        await _context.SaveChangesAsync();

        return leader;
    }

    public Task<Leader> GetLeaderByParticipantIdAsync(Guid participantId)
    {
        return _context.Leaders.FirstAsync(x => x.ParticipantId == participantId);
    }

    public async Task<Leader> UpdateLeaderAsync(Guid participantId, Guid checkpointId, Guid segmentId, Guid checkinId, uint overallTime, uint overallPace, uint nextPredictedSegmentTime)
    {
        var leader = await GetLeaderByParticipantIdAsync(participantId);
        leader.LastCheckpointId = checkpointId;
        leader.LastSegmentId = segmentId;
        leader.LastCheckinId = checkinId;
        leader.OverallTime = overallTime;
        leader.OverallPace = overallPace;
        leader.NextPredictedSegmentTime = nextPredictedSegmentTime;

        await _context.SaveChangesAsync();
        return leader;
    }

    public async Task<List<Leader>> GetLeadersAsync()
    {
        return await _context.Leaders
            .Include(x => x.Participant)
            .Include(x => x.LastSegment)
            .Include(x => x.LastCheckpoint)
            .Include(x => x.LastCheckin)
            .OrderByDescending(x => x.LastSegment.TotalDistance)
            .ThenBy(x => x.OverallTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Leader>> GetLeadersByRaceIdAsync(Guid raceId)
    {
        return await _context.Leaders
            .Where(x => x.Participant!.RaceId == raceId)
            .Include(x => x.Participant)
            .Include(x => x.LastSegment)
            .Include(x => x.LastCheckpoint)
            .Include(x => x.LastCheckin)
            .OrderByDescending(x => x.LastSegment.TotalDistance)
            .ThenBy(x => x.OverallTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Leader>> GetLeadersByWatcherUserAsync(string userId)
    {
        var watchers = await _context.Watchers
            .Where(w => w.UserId == userId)
            .AsNoTracking()
            .Select(w => w.ParticipantId)
            .ToListAsync();

        return await _context.Leaders
            .Include(l => l.LastCheckin)
            .Include(l => l.LastSegment)
            .Include(l => l.Participant)
            .AsNoTracking()
            .Where(l => l.Participant.Race.Active == true && watchers.Contains(l.ParticipantId))
            .OrderByDescending(l => l.LastSegment.TotalDistance)
            .ThenBy(l => l.LastCheckin.When)
            .ToListAsync();
    }

    public async Task<List<Leader>> GetLeadersByDroppedStatusAsync()
    {
        return await _context.Leaders
            .Include(l => l.LastCheckin)
            .Include(l => l.LastSegment)
            .Include(l => l.Participant)
            .Where(l => l.Participant.Race.Active == true && (l.Participant.Status == Status.DNF || l.Participant.Status == Status.DNS))
            .OrderBy(l => l.Participant.Bib)
            .ThenBy(l => l.LastCheckin.When)
            .AsNoTracking()
            .ToListAsync();
    }
}