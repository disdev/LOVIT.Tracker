using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Services;
using LOVIT.Tracker.Utilities;

namespace LOVIT.Tracker.Models;

public interface ISeedService
{
    Task Initialize();
    Task SimulateRace(Guid raceId, int numberOfParticipants, bool addCheckins, int hours);
}

public class SeedService : ISeedService
{
    private readonly TrackerContext _context;
    private readonly IMessageService _messageService;
    private readonly ICheckinService _checkinService;
    private readonly IParticipantService _participantService;
    private readonly IRaceService _raceService;

    public SeedService(TrackerContext context, IMessageService messageService, ICheckinService checkinService, IParticipantService participantService, IRaceService raceService)
    {
        _context = context;
        _messageService = messageService;
        _checkinService = checkinService;
        _participantService = participantService;
        _raceService = raceService;
    }

    public async Task Initialize()
    {
        if (_context.Races.Any())
        {
            await EmptyDatabase();
            //return;
        }

        var races = AddRaces();
        var checkpoints = AddCheckpoints();
        var segments = AddSegments(races, checkpoints);
        var monitors = AddMonitors(checkpoints);
    }

    public async Task SimulateRace(Guid raceId, int numberOfParticipants, bool addCheckins, int hours)
    {
        var race = await _context.Races.Where(x => x.Id == raceId).Include(x => x.Segments).FirstAsync();
        var participants = await AddParticipants(race, numberOfParticipants);

        if (addCheckins)
        {
            await _raceService.StartRace("+1111111111", race.Code, DateTime.UtcNow.AddHours(hours * -1));
            var result = await AddCheckins(race, participants, (hours > 0), hours);
        }
    }

    private List<Race> AddRaces()
    {
        var e1 = new RaceEvent()
        {
            Id = Guid.NewGuid(),
            Name = "LOVIT",
            Location = "Lake Ouachita",
            Start = new DateTime(2023, 2, 24, 17, 0, 0).ToUniversalTime(),
            Current = true
        };
        _context.RaceEvents.Add(e1);
        _context.SaveChanges();

        var r1 = new Race()
        {
            Id = Guid.NewGuid(),
            Name = "100 Mile",
            Code = "100M",
            Distance = 100.0F,
            Start = new DateTime(2024, 2, 23, 23, 0, 0),
            End = new DateTime(2024, 2, 25, 9, 0, 0),
            UltraSignupUrl = "https://ultrasignup.com/entrants_event.aspx?did=107333",
            RaceEvent = e1,
            Segments = new List<Segment>(),
            Participants = new List<Participant>(),
            Active = true
        };

        var r2 = new Race()
        {
            Id = Guid.NewGuid(),
            Name = "100 Kilometer",
            Code = "100K",
            Distance = 100.0F,
            Start = new DateTime(2024, 2, 24, 12, 0, 0),
            End = new DateTime(2024, 2, 25, 9, 0, 0),
            UltraSignupUrl = "https://ultrasignup.com/entrants_event.aspx?did=107334",
            RaceEvent = e1,
            Segments = new List<Segment>(),
            Participants = new List<Participant>(),
            Active = true
        };

        var races = new List<Race>() { r1, r2 };
        _context.Races.AddRange(races);
        _context.SaveChanges();

        return races;
    }

    private List<Checkpoint> AddCheckpoints()
    {
        var c1 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "East Cove Pavilion",
            GeoJson = @"{""type"": ""Feature"", ""properties"": { ""name"": ""East Cove Pavilion"" }, ""geometry"": { ""coordinates"": [ -93.43124485906752, 34.57632124535455 ], ""type"": ""Point"" } }",
            Number = 0,
            Code = ""
        };

        var c2 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Hickory Nut Mountain",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Hickory Nut Mountain"" }, ""geometry"": { ""coordinates"": [ -93.40772635215696, 34.56413017495403 ], ""type"": ""Point"" } }",
            Number = 1,
            Code = ""
        };

        var c3 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Joplin Road Crossing",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Joplin"" }, ""geometry"": { ""coordinates"": [ -93.45197553145184, 34.54434898546012 ], ""type"": ""Point"" } }",
            Number = 2,
            Code = ""
        };

        var c4 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Tompkins Bend",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Tompkins Bend"" }, ""geometry"": { ""coordinates"": [ -93.47078789234095, 34.568732403410976 ], ""type"": ""Point"" } }",
            Number = 3,
            Code = ""
        };

        var c5 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "ADA",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""ADA"" }, ""geometry"": { ""coordinates"": [ -93.47250300454692, 34.543646238287565 ], ""type"": ""Point"" } }",
            Number = 4,
            Code = ""
        };

        var c6 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Forest Road 47A",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""47A"" }, ""geometry"": { ""coordinates"": [ -93.38906785603287, 34.54481133064378 ], ""type"": ""Point"" } }",
            Number = 5,
            Code = ""
        };

        var c7 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Charlton",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Charlton"" }, ""geometry"": { ""coordinates"": [ -93.38316305494537, 34.51599072029107 ], ""type"": ""Point"" } }",
            Number = 6,
            Code = ""
        };

        var c8 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Crystal Springs",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Crystal Springs"" }, ""geometry"": { ""coordinates"": [ -93.3514341198992, 34.54138810542142 ], ""type"": ""Point"" } }",
            Number = 7,
            Code = ""
        };

        var c9 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Bear Creek",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Bear Creek"" }, ""geometry"": { ""coordinates"": [ -93.3423075843046, 34.54804122506539 ], ""type"": ""Point"" } }",
            Number = 8,
            Code = ""
        };

        var c10 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Brady Mountain Road",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Brady Mountain Road"" }, ""geometry"": { ""coordinates"": [ -93.2603652796786, 34.558186497621435 ], ""type"": ""Point"" } }",
            Number = 9,
            Code = ""
        };

        var c11 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Spillway ",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Spillway"" }, ""geometry"": { ""coordinates"": [ -93.22230412488346, 34.567531655456946 ], ""type"": ""Point"" } }",
            Number = 10,
            Code = ""
        };

        var c12 = new Checkpoint()
        {
            Id = Guid.NewGuid(),
            Name = "Avery Rec Area",
            GeoJson = @"{ ""type"": ""Feature"", ""properties"": { ""name"": ""Avery"" }, ""geometry"": { ""coordinates"": [ -93.19212063798282, 34.568943018340605 ], ""type"": ""Point"" } }",
            Number = 11,
            Code = ""
        };

        var checkpoints = new List<Checkpoint> {
c1, c2, c3, c4, c5, c6, c7, c8, c9, c10,
c11, c12
};
        _context.Checkpoints.AddRange(checkpoints);
        _context.SaveChanges();
        return checkpoints;
    }

    private List<Segment> AddSegments(List<Race> races, List<Checkpoint> checkpoints)
    {
        var s1 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Start to Hickory Nut Mountain",
            GeoJson = @"segment-1.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 0),
            ToCheckpoint = checkpoints.First(x => x.Number == 1),
            Order = 1,
            Distance = 4.25,
            TotalDistance = 4.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s2 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Hickory Nut Mountain to Joplin Road Crossing",
            GeoJson = @"segment-2.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 1),
            ToCheckpoint = checkpoints.First(x => x.Number == 2),
            Order = 2,
            Distance = 5.25,
            TotalDistance = 9.5,
            Race = races.First(x => x.Code == "100M")
        };

        var s3 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Joplin Road Crossing to Tompkins Bend",
            GeoJson = @"segment-3.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 2),
            ToCheckpoint = checkpoints.First(x => x.Number == 3),
            Order = 3,
            Distance = 4.75,
            TotalDistance = 14.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s4 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Tompkins Bend to ADA",
            GeoJson = @"segment-4.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 3),
            ToCheckpoint = checkpoints.First(x => x.Number == 4),
            Order = 4,
            Distance = 5.5,
            TotalDistance = 19.75,
            Race = races.First(x => x.Code == "100M")
        };

        var s5 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "ADA to Tompkins Bend",
            GeoJson = @"segment-5.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 4),
            ToCheckpoint = checkpoints.First(x => x.Number == 3),
            Order = 5,
            Distance = 5.5,
            TotalDistance = 25.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s6 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Tompkins Bend to Joplin Road Crossing",
            GeoJson = @"segment-6.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 3),
            ToCheckpoint = checkpoints.First(x => x.Number == 2),
            Order = 6,
            Distance = 4.75,
            TotalDistance = 30,
            Race = races.First(x => x.Code == "100M")
        };

        var s7 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Joplin Road Crossing to Hickory Nut Mountain",
            GeoJson = @"segment-7.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 2),
            ToCheckpoint = checkpoints.First(x => x.Number == 1),
            Order = 7,
            Distance = 5.25,
            TotalDistance = 35.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s8 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Hickory Nut Mountain to Forest Road 47A",
            GeoJson = @"segment-8.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 1),
            ToCheckpoint = checkpoints.First(x => x.Number == 5),
            Order = 8,
            Distance = 3.5,
            TotalDistance = 38.75,
            Race = races.First(x => x.Code == "100M")
        };

        var s9 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Forest Road 47A to Charlton",
            GeoJson = @"segment-9.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 5),
            ToCheckpoint = checkpoints.First(x => x.Number == 6),
            Order = 9,
            Distance = 4.5,
            TotalDistance = 43.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s10 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Charlton to Crystal Springs",
            GeoJson = @"segment-10.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 6),
            ToCheckpoint = checkpoints.First(x => x.Number == 7),
            Order = 10,
            Distance = 4.5,
            TotalDistance = 47.75,
            Race = races.First(x => x.Code == "100M")
        };

        var s11 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Crystal Springs to Bear Creek",
            GeoJson = @"segment-11.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 7),
            ToCheckpoint = checkpoints.First(x => x.Number == 8),
            Order = 11,
            Distance = 3.5,
            TotalDistance = 51.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s12 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Bear Creek to Brady Mountain Road",
            GeoJson = @"segment-12.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 8),
            ToCheckpoint = checkpoints.First(x => x.Number == 9),
            Order = 12,
            Distance = 7.5,
            TotalDistance = 58.75,
            Race = races.First(x => x.Code == "100M")
        };

        var s13 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Brady Mountain Road to Spillway",
            GeoJson = @"segment-13.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 9),
            ToCheckpoint = checkpoints.First(x => x.Number == 10),
            Order = 13,
            Distance = 3.75,
            TotalDistance = 62.5,
            Race = races.First(x => x.Code == "100M")
        };

        var s14 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Spillway to Avery Rec Area",
            GeoJson = @"segment-14.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 10),
            ToCheckpoint = checkpoints.First(x => x.Number == 11),
            Order = 14,
            Distance = 3,
            TotalDistance = 65.5,
            Race = races.First(x => x.Code == "100M")
        };

        var s15 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Avery Rec Area to Spillway",
            GeoJson = @"segment-15.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 11),
            ToCheckpoint = checkpoints.First(x => x.Number == 10),
            Order = 15,
            Distance = 3,
            TotalDistance = 68.5,
            Race = races.First(x => x.Code == "100M")
        };

        var s16 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Spillway to Brady Mountain Road",
            GeoJson = @"segment-16.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 10),
            ToCheckpoint = checkpoints.First(x => x.Number == 9),
            Order = 16,
            Distance = 3.75,
            TotalDistance = 72.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s17 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Brady Mountain Road to Bear Creek",
            GeoJson = @"segment-17.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 9),
            ToCheckpoint = checkpoints.First(x => x.Number == 8),
            Order = 17,
            Distance = 7.5,
            TotalDistance = 79.75,
            Race = races.First(x => x.Code == "100M")
        };

        var s18 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Bear Creek to Crystal Springs",
            GeoJson = @"segment-18.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 8),
            ToCheckpoint = checkpoints.First(x => x.Number == 7),
            Order = 18,
            Distance = 3.5,
            TotalDistance = 83.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s19 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Crystal Springs to Charlton",
            GeoJson = @"segment-19.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 7),
            ToCheckpoint = checkpoints.First(x => x.Number == 6),
            Order = 19,
            Distance = 4.5,
            TotalDistance = 87.75,
            Race = races.First(x => x.Code == "100M")
        };

        var s20 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Charlton to Forest Road 47A",
            GeoJson = @"segment-20.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 6),
            ToCheckpoint = checkpoints.First(x => x.Number == 5),
            Order = 20,
            Distance = 4.5,
            TotalDistance = 92.25,
            Race = races.First(x => x.Code == "100M")
        };

        var s21 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Forest Road 47A to Hickory Nut Mountain",
            GeoJson = @"segment-21.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 5),
            ToCheckpoint = checkpoints.First(x => x.Number == 1),
            Order = 21,
            Distance = 3.5,
            TotalDistance = 95.75,
            Race = races.First(x => x.Code == "100M")
        };

        var s22 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Hickory Nut Mountain to Finish",
            GeoJson = @"segment-22.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 1),
            ToCheckpoint = checkpoints.First(x => x.Number == 0),
            Order = 22,
            Distance = 4.25,
            TotalDistance = 100,
            Race = races.First(x => x.Code == "100M")
        };



        var s23 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Start to Hickory Nut Mountain",
            GeoJson = @"segment-1.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 0),
            ToCheckpoint = checkpoints.First(x => x.Number == 1),
            Order = 1,
            Distance = 4.25,
            TotalDistance = 4.25,
            Race = races.First(x => x.Code == "100K")
        };
        var s24 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Hickory Nut Mountain to Forest Road 47A",
            GeoJson = @"segment-2.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 1),
            ToCheckpoint = checkpoints.First(x => x.Number == 5),
            Order = 2,
            Distance = 3.5,
            TotalDistance = 7.75,
            Race = races.First(x => x.Code == "100K")
        };

        var s25 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Forest Road 47A to Crystal Springs",
            GeoJson = @"segment-3.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 5),
            ToCheckpoint = checkpoints.First(x => x.Number == 7),
            Order = 3,
            Distance = 4,
            TotalDistance = 11.75,
            Race = races.First(x => x.Code == "100K")
        };

        var s26 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Crystal Springs to Bear Creek",
            GeoJson = @"segment-4.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 7),
            ToCheckpoint = checkpoints.First(x => x.Number == 8),
            Order = 4,
            Distance = 3.5,
            TotalDistance = 15.25,
            Race = races.First(x => x.Code == "100K")
        };

        var s27 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Bear Creek to Brady Mountain Road",
            GeoJson = @"segment-5.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 8),
            ToCheckpoint = checkpoints.First(x => x.Number == 9),
            Order = 5,
            Distance = 7.5,
            TotalDistance = 22.75,
            Race = races.First(x => x.Code == "100K")
        };

        var s28 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Brady Mountain Road to Spillway",
            GeoJson = @"segment-6.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 9),
            ToCheckpoint = checkpoints.First(x => x.Number == 10),
            Order = 6,
            Distance = 3.75,
            TotalDistance = 36.25,
            Race = races.First(x => x.Code == "100K")
        };

        var s29 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Spillway to Avery Rec Area",
            GeoJson = @"segment-7.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 10),
            ToCheckpoint = checkpoints.First(x => x.Number == 11),
            Order = 7,
            Distance = 3,
            TotalDistance = 29.5,
            Race = races.First(x => x.Code == "100K")
        };

        var s30 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Avery Rec Area to Spillway",
            GeoJson = @"segment-8.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 11),
            ToCheckpoint = checkpoints.First(x => x.Number == 10),
            Order = 8,
            Distance = 3,
            TotalDistance = 32.5,
            Race = races.First(x => x.Code == "100K")
        };

        var s31 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Spillway to Brady Mountain Road",
            GeoJson = @"segment-9.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 10),
            ToCheckpoint = checkpoints.First(x => x.Number == 9),
            Order = 9,
            Distance = 3.75,
            TotalDistance = 36.25,
            Race = races.First(x => x.Code == "100K")
        };

        var s32 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Brady Mountain Road to Bear Creek",
            GeoJson = @"segment-10.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 9),
            ToCheckpoint = checkpoints.First(x => x.Number == 8),
            Order = 10,
            Distance = 7.5,
            TotalDistance = 43.75,
            Race = races.First(x => x.Code == "100K")
        };

        var s33 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Bear Creek to Crystal Springs",
            GeoJson = @"segment-11.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 8),
            ToCheckpoint = checkpoints.First(x => x.Number == 7),
            Order = 11,
            Distance = 3.5,
            TotalDistance = 47.25,
            Race = races.First(x => x.Code == "100K")
        };

        var s34 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Crystal Springs to Forest Road 47A",
            GeoJson = @"segment-12.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 7),
            ToCheckpoint = checkpoints.First(x => x.Number == 5),
            Order = 12,
            Distance = 4,
            TotalDistance = 51.25,
            Race = races.First(x => x.Code == "100K")
        };

        var s35 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Forest Road 47A to Hickory Nut Mountain",
            GeoJson = @"segment-13.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 5),
            ToCheckpoint = checkpoints.First(x => x.Number == 1),
            Order = 13,
            Distance = 3.5,
            TotalDistance = 53.75,
            Race = races.First(x => x.Code == "100K")
        };

        var s36 = new Segment()
        {
            Id = Guid.NewGuid(),
            Name = "Hickory Nut Mountain to Finish",
            GeoJson = @"segment-14.json",
            FromCheckpoint = checkpoints.First(x => x.Number == 1),
            ToCheckpoint = checkpoints.First(x => x.Number == 0),
            Order = 14,
            Distance = 4.25,
            TotalDistance = 59,
            Race = races.First(x => x.Code == "100K")
        };

        var r1Segments = new List<Segment>()
{
s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16, s17, s18, s19, s20, s21, s22
};

        var r2Segments = new List<Segment>()
{
s23, s24, s25, s26, s27, s28, s29, s30, s31, s32, s33, s34, s35, s36
};

        _context.Segments.AddRange(r1Segments);
        _context.Segments.AddRange(r2Segments);
        _context.SaveChanges();

        var segments = new List<Segment>();
        segments.AddRange(r1Segments);
        segments.AddRange(r2Segments);

        return segments;
    }

    private List<Monitor> AddMonitors(List<Checkpoint> checkpoints)
    {
        var monitors = new List<Monitor>();

        foreach (var checkpoint in _context.Checkpoints)
        {
            monitors.Add(new Monitor()
            {
                Checkpoint = checkpoint,
                Name = "John Doe",
                PhoneNumber = "+1111111111",
                Active = true
            });
        }

        _context.Monitors.AddRange(monitors);
        _context.SaveChanges();

        return monitors;
    }

    private async Task<List<Participant>> AddParticipants(Race race, int numberOfParticipants = 10)
    {
        var participants = new List<Participant>();

        for (int i = 1; i <= numberOfParticipants; i++)
        {
            var bibStart = (race.Code == "100M") ? 100 : 1000;
            var participant = new Participant()
            {
                Id = Guid.NewGuid(),
                Bib = $"{Convert.ToString(bibStart + i)}",
                FirstName = "Person",
                LastName = $"{Convert.ToString(bibStart + i)}",
                City = "Anywhere",
                Region = "US",
                Age = "1-100",
                Gender = Gender.Male,
                Race = race,
                Status = Status.Registered,
                PictureUrl = "empty",
                UltraSignupEmail = "email"
            };
            participant = await _participantService.AddOrUpdateParticipantAsync(participant);

            participants.Add(participant);
        }

        return participants;
    }

    private async Task<int> AddCheckins(Race race, List<Participant> participants, bool limit = false, int hourLimit = 6)
    {
        var random = new Random();
        var raceSegments = race.Segments.OrderBy(x => x.Order).ToList();

        foreach (var participant in participants)
        {
            var quality = random.Next(1, 10);
            DateTime lastCheckinTime = race.Start;

            participant.Status = Status.Started;

            foreach (var segment in raceSegments)
            {
                DateTime when = CalculateWhen(segment.Distance, quality, lastCheckinTime);
                uint elapsed = (uint)(when - lastCheckinTime).TotalSeconds;

                if (limit == false || when <= race.Start.AddHours(hourLimit))
                {
                    var message = await _messageService.AddMessageAsync(new Message()
                    {
                        Id = Guid.NewGuid(),
                        From = "+1111111111",
                        Body = participant.Bib,
                        FromCity = "NONE",
                        FromState = "NONE",
                        FromZip = "NON",
                        FromCountry = "NONE",
                        Received = when
                    });

                    var handleResult = await _messageService.HandleMessageAsync(message);

                    lastCheckinTime = when;
                }
                else
                {
                    break;
                }
            }
        }

        return 1;
    }

    private DateTime CalculateWhen(double distance, int quality, DateTime last)
    {
        Random rnd = new Random();

        int mins = rnd.Next(8 + quality, 14 + quality);
        int secs = rnd.Next(0, 59);

        int pace = mins * 60 + secs;
        double elapsed = pace * distance;

        return last.AddSeconds(elapsed);
    }

    public async Task EmptyDatabase()
    {
        await _context.Checkins.ExecuteDeleteAsync();
        await _context.Watchers.ExecuteDeleteAsync();
        await _context.Messages.ExecuteDeleteAsync();
        await _context.Participants.ExecuteDeleteAsync();

        await _context.Checkpoints.ExecuteDeleteAsync();
        await _context.Segments.ExecuteDeleteAsync();
        await _context.Monitors.ExecuteDeleteAsync();
        await _context.Checkins.ExecuteDeleteAsync();
        await _context.Races.ExecuteDeleteAsync();
        await _context.RaceEvents.ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }
}