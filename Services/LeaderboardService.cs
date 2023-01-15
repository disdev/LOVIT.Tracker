/*
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Utilities;

namespace LOVIT.Tracker.Services;

public interface ILeaderboardService
{
    Task<List<Participant>> GetLeadersAsync();
    Task<List<Participant>> GetLeadersByRaceAsync(Guid raceId);
    Task<List<Participant>> GetLeadersByWatcherUserAsync(string userId);
    Task GenerateLeaderboardsAsync();
    Task GenerateLeaderboardAsync(Guid raceId);
    Task<List<Leaderboard>> GetLeaderboardsAsync();
    Task<Leaderboard> GetLeaderboardAsync(Guid raceId);
}

public class LeaderboardService
{
    private readonly TrackerContext _context;
    private readonly IRaceService _raceService;

    public LeaderboardService(IRaceService raceService, TrackerContext context)
    {
        _context = context;
        _raceService = raceService;
    }

    public async Task<List<Participant>> GetLeadersAsync()
    {
        return await _context.Participants
            .Include(x => x.LastCheckin)
            .Include(x => x.LastCheckpoint)
            .Include(x => x.LastSegment)
            .Where(x => x.Race!.Active == true)
            .OrderByDescending(x => x.LastSegment.TotalDistance)
            .ThenBy(x => x.LastCheckin.When)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Leaderboard> GetLeaderboardAsync(Guid raceId)
    {
        return await _context.Leaderboards
            .Where(x => x.Race!.Id == raceId)
            .OrderByDescending(x => x.Updated)
            .Take(1)
            .SingleAsync();
    }

    public async Task<List<Leaderboard>> GetLeaderboardsAsync()
    {
        var leaderboards = new List<Leaderboard>();
        var results = await _context.Leaderboards
            .GroupBy(x => x.Race)
            .Select(g => g.OrderByDescending(x => x.Updated).Take(1))
            .ToListAsync();

        results.ForEach(x => leaderboards.Add(x.First()));
        return leaderboards;
    }

    public async Task GenerateLeaderboardsAsync()
    {
        var races = await _raceService.GetRacesAsync();
        var leaders = await GetLeadersAsync();
        
        foreach (var race in races)
        {
            await GenerateLeaderboardAsync(race.Id);
        }
    }

    public async Task GenerateLeaderboardAsync(Guid raceId)
    {
        var race = await _raceService.GetRaceAsync(raceId);
        var participants = await GetLeadersByRaceAsync(raceId);
        var leaderboard = new Leaderboard();
        leaderboard.Race = race;
        leaderboard.Updated = DateTime.UtcNow;
        leaderboard.LeaderboardEntries = new List<LeaderboardEntry>();

        var leaderPlace = 1;
        foreach (var participant in participants.Where(x => x.RaceId == race.Id))
        {
            leaderboard.LeaderboardEntries.Add(new LeaderboardEntry(participant, leaderPlace));
            leaderPlace++;
        }

        await _context.Leaderboards.AddAsync(leaderboard);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Participant>> GetLeadersByRaceAsync(Guid raceId)
    {
        return await _context.Participants
            .Include(x => x.LastCheckin)
            .Include(x => x.LastCheckpoint)
            .Include(x => x.LastSegment)
            .AsNoTracking()
            .Where(x => x.Race.Active == true && x.RaceId == raceId)
            .OrderByDescending(x => x.LastSegment.TotalDistance)
            .ThenBy(x => x.LastCheckin.When)
            .ToListAsync();
    }

    public async Task<List<Participant>> GetLeadersByWatcherUserAsync(string userId)
    {
        var watchers = await _context.Watchers
            .Where(w => w.UserId == userId)
            .AsNoTracking()
            .Select(w => w.ParticipantId)
            .ToListAsync();

        return await _context.Participants
            .Include(x => x.LastCheckin)
            .Include(x => x.LastCheckpoint)
            .Include(x => x.LastSegment)
            .AsNoTracking()
            .Where(x => x.Race.Active == true && watchers.Contains(x.Id))
            .OrderByDescending(x => x.LastSegment.TotalDistance)
            .ThenBy(x => x.LastCheckin.When)
            .ToListAsync();
    }
}
*/