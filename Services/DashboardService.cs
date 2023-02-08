using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace LOVIT.Tracker.Services;

public interface IDashboardService
{
    Task<int> GetParticipantsByRaceAndStatusAsync(Guid raceId, Status status);
    Task<int> GetMessageCountAsync();
    Task<int> GetCheckinCountAsync();
    Task<int> GetPendingCheckinCountAsync();
    Task<int> GetMonitorCountAsync();
    Task<int> GetWatcherCountAsync();
    
}

public class DashboardService : IDashboardService
{
    private readonly TrackerContext _context;

    public DashboardService(TrackerContext context)
    {
        _context = context;
    }

    public async Task<int> GetCheckinCountAsync()
    {
        return await _context.Checkins.CountAsync(x => x.Confirmed == true);
    }

    public async Task<int> GetPendingCheckinCountAsync()
    {
        return await _context.Checkins.CountAsync(x => x.Confirmed == false);
    }

    public async Task<int> GetMessageCountAsync()
    {
        return await _context.Messages.CountAsync();
    }

    public async Task<int> GetParticipantsByRaceAndStatusAsync(Guid raceId, Status status)
    {
        return await _context.Participants.CountAsync(x => x.RaceId == raceId && x.Status == status);
    }

    public async Task<int> GetMonitorCountAsync()
    {
        return await _context.Monitors.CountAsync();
    }

    public async Task<int> GetWatcherCountAsync()
    {
        return await _context.Watchers.CountAsync();
    }
}