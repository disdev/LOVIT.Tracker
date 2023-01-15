using System;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Services;

public interface ICheckpointService
{
    Task<List<Checkpoint>> GetCheckpointsAsync();
    Task<Checkpoint> GetCheckpointAsync(Guid checkpointId);
}

public class CheckpointService : ICheckpointService
{
    private readonly TrackerContext _context;

    public CheckpointService(TrackerContext context)
    {
        _context = context;
    }

    public async Task<List<Checkpoint>> GetCheckpointsAsync()
    {
        return await _context.Checkpoints.OrderBy(x => x.Number).ToListAsync();
    }

    public async Task<Checkpoint> GetCheckpointAsync(Guid checkpointId)
    {
        return await _context.Checkpoints.Where(x => x.Id == checkpointId).FirstAsync();
    }
}