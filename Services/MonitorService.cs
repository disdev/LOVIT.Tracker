using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;

namespace LOVIT.Tracker.Services;

public interface IMonitorService
{
    Task<List<LOVIT.Tracker.Models.Monitor>> GetMonitorsAsync();
    Task<LOVIT.Tracker.Models.Monitor> GetMonitorAsync(Guid monitorId);
    Task<List<LOVIT.Tracker.Models.Monitor>> GetMonitorsForCheckpointAsync(Guid checkpointId);
    Task<bool> IsValidMonitor(string phoneNumber, Int16 checkpoint = -1);
    Task<List<LOVIT.Tracker.Models.Monitor>> GetMonitorsForPhoneNumberAsync(string phoneNumber);
    Task<LOVIT.Tracker.Models.Monitor> AddMonitor(string phoneNumber, Int16 checkpointNumber);
    Task<int> GetMissingMonitorCountAsync();
}

public class MonitorService : IMonitorService
{
    private readonly TrackerContext _context;

    public MonitorService(TrackerContext context)
    {
        _context = context;
    }

    public async Task<List<LOVIT.Tracker.Models.Monitor>> GetMonitorsAsync()
    {
        return await _context.Monitors.ToListAsync();
    }

    public async Task<LOVIT.Tracker.Models.Monitor> GetMonitorAsync(Guid monitorId)
    {
        return await _context.Monitors.Where(x => x.Id == monitorId).FirstAsync();
    }

    public async Task<List<LOVIT.Tracker.Models.Monitor>> GetMonitorsForCheckpointAsync(Guid checkpointId)
    {
        return await _context.Monitors.Where(x => x.CheckpointId == checkpointId).ToListAsync();
    }

    public async Task<bool> IsValidMonitor(string phoneNumber, Int16 checkpoint = -1)
    {
        if (checkpoint == -1)
        {
            return await _context.Monitors.Where(x => x.PhoneNumber == phoneNumber).AnyAsync();    
        }

        return await _context.Monitors.Where(x => x.PhoneNumber == phoneNumber && x.Checkpoint.Number == checkpoint).AnyAsync();
    }

    public async Task<LOVIT.Tracker.Models.Monitor> AddMonitor(string phoneNumber, Int16 checkpointNumber)
    {
        if (_context.Checkpoints.Where(x => x.Number == checkpointNumber).Any())
        {
            var monitor = await _context.Monitors.Where(x => x.PhoneNumber == phoneNumber && x.Checkpoint.Number == checkpointNumber).SingleOrDefaultAsync();
            var checkpoint = await _context.Checkpoints.Where(x => x.Number == checkpointNumber).SingleAsync();

            if (monitor is null)
            {
                monitor = new LOVIT.Tracker.Models.Monitor()
                {
                    Id = Guid.NewGuid(),
                    Name = "",
                    PhoneNumber = phoneNumber,
                    Active = true,
                    Checkpoint = checkpoint
                };
                
                _context.Monitors.Add(monitor);
                await _context.SaveChangesAsync();
            }

            return monitor;
        }

        return new LOVIT.Tracker.Models.Monitor();
    }

    public async Task<List<LOVIT.Tracker.Models.Monitor>> GetMonitorsForPhoneNumberAsync(string phoneNumber)
    {
        return await _context.Monitors
            .Where(x=> x.PhoneNumber == phoneNumber)
            .OrderBy(x => x.Checkpoint.Number)
            .Include(x => x.Checkpoint)
            .ToListAsync();
    }

    public async Task<int> GetMissingMonitorCountAsync()
    {
        return await _context.Checkpoints
            .Where(x => x.Monitors.Count() == 0)
            .CountAsync();
    }
}