using System;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Services;

public interface ISegmentService
{
    Task<List<Segment>> GetSegmentsAsync();
    Task<List<Segment>> GetSegmentsAsync(Guid raceId);
    Task<Segment> GetSegmentAsync(Guid segmentId);
    Task<Segment> GetSegmentAsync(Int16 order, Guid raceId);
    Task<Segment> GetFinishSegment(Guid raceId);
}

public class SegmentService : ISegmentService
{
    private readonly TrackerContext _context;

    public SegmentService(TrackerContext context)
    {
        _context = context;
    }

    public async Task<List<Segment>> GetSegmentsAsync()
    {
        return await _context.Segments.OrderBy(x => x.RaceId).ThenBy(x => x.Order).ToListAsync();
    }
    
    public async Task<List<Segment>> GetSegmentsAsync(Guid raceId)
    {
        return await _context.Segments
            .Where(x => x.RaceId == raceId)
            .Include(x => x.FromCheckpoint)
            .Include(x => x.ToCheckpoint)
            .OrderBy(x => x.Order)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Segment> GetSegmentAsync(Guid segmentId)
    {
        return await _context.Segments.Where(x => x.Id == segmentId).SingleAsync();
    }

    public async Task<Segment> GetFinishSegment(Guid raceId)
    {
        return await _context.Segments.Where(x => x.RaceId == raceId && x.ToCheckpoint.Number == 0).SingleAsync();
    }

    public async Task<Segment> GetSegmentAsync(Int16 order, Guid raceId)
    {
        return await _context.Segments.Where(x => x.Order == order && x.RaceId == raceId).SingleAsync();
    }
}