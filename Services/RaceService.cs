using System;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Services;

public interface IRaceService
{
    Task<List<Race>> GetRacesAsync();
    Task<Race> GetRaceAsync(Guid raceId);
    Task<Race> GetRaceAsync(string raceCode);
    Task<Race> StartRace(string from, string raceCode, DateTime when);
    Task<List<Race>> GetRacesWithSegmentsAsync();
    Task<List<Race>> GetRacesWithSegmentsAndCheckpointsAsync();
    Task<List<Race>> GetRacesFromCheckpointAsync(Guid checkpointId);
}

public class RaceService : IRaceService
{
    private readonly TrackerContext _context;

    private readonly IMonitorService _monitorService;
    private readonly IParticipantService _participantService;
    private readonly ITwilioService _twilioService;

    public RaceService(TrackerContext context, IMonitorService monitorService, IParticipantService participantService, ITwilioService twilioService)
    {
        _context = context;
        _monitorService = monitorService;
        _participantService = participantService;
        _twilioService = twilioService;
    }

    public async Task<List<Race>> GetRacesAsync()
    {
        return await _context.Races.Where(x => x.Active == true).OrderBy(x => x.Start).ToListAsync();
    }

    public async Task<List<Race>> GetRacesWithSegmentsAsync()
    {
        return await _context.Races.Include(x => x.Segments).Where(x => x.Active == true).ToListAsync();
    }

    public async Task<List<Race>> GetRacesWithSegmentsAndCheckpointsAsync()
    {
        return await _context.Races
            .Include(x => x.Segments)
                .ThenInclude(x => x.ToCheckpoint)
            .Include(x => x.Segments)
                .ThenInclude(x => x.FromCheckpoint)
            .Where(x => x.Active == true)
            .ToListAsync();
    }

    public async Task<Race> GetRaceAsync(Guid raceId)
    {
        return await _context.Races.Where(x => x.Id == raceId).FirstAsync();
    }

    public async Task<Race> GetRaceAsync(string raceCode)
    {
        if (Guid.TryParse(raceCode, out var raceId))
        {
            return await GetRaceAsync(raceId);
        }
        return await _context.Races.Where(x => x.Code == raceCode).FirstAsync();
    }

    public async Task<Race> StartRace(string from, string raceCode, DateTime when)
    {
        if (await _monitorService.IsValidMonitor(from, 0)) // 0 is the start checkpoint
        {
            var race = await GetRaceAsync(raceCode);
            race.Start = when;
            
            await _context.SaveChangesAsync();
            await _context.Database.ExecuteSqlRawAsync("update Participants set Status = 2 where Status = 0 and RaceId = {0}", race.Id);

            try
            {
                var watchers = await _context.Watchers.Where(x => x.Participant.RaceId == race.Id).ToListAsync();
                foreach (var watcher in watchers)
                {
                    await _twilioService.SendMessageAsync(watchers, $"{watcher.Participant.FullName} has started the LOVIT {race.Code}.");
                }
            }
            catch (Exception ex)
            {
                // if the twilio service fails, keep going
                Console.WriteLine("Error sending watched messages for race start!");
                Console.WriteLine(ex.ToString());
            }
            
            return race;
        }
        else
        {
            throw new InvalidOperationException("Something went wrong starting the race!");
        }            
    }

    public async Task<List<Race>> GetRacesFromCheckpointAsync(Guid checkpointId)
    {
        return await _context.Races
            .Include(x => x.Segments)
            .Where(x => x.Active == true)
            .Where(x => x.Segments.Any(y => y.ToCheckpointId == checkpointId)).ToListAsync();
    }
}