using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Utilities;

namespace LOVIT.Tracker.Services;

public interface IParticipantService
{
    Task<List<Participant>> GetParticipantsAsync();
    Task<List<Participant>> GetParticipantsAsync(Guid raceId);
    Task<List<Participant>> GetParticipantsAsync(string raceCode);
    Task<Participant> GetParticipantAsync(string id, bool includeRace = false);
    Task<Participant> GetParticipantFromLinkCodeAsync(Guid linkCode);
    Task<Participant> GetParticipantAsync(Guid participantId, bool includeRace = false);
    Task<Participant> GetParticipantWithCheckinsAsync(Guid participantId, bool includeRace = false);
    Task<Participant> AddOrUpdateParticipantAsync(Participant participant);
    Task<Participant> AddParticipantAsync(Participant participant);
    Task<Participant> UpdateParticipantAsync(Participant participant);
    Task<Participant> SetParticipantStatusAsync(Guid participantId, Status status);
    Task<Participant> LinkParticipantToUserIdAsync(Guid participantId, string userId, string profileImageUrl = "");
    Task UpdateParticipantProfileImageUrlAsync(Guid participantId, string profileImageUrl);
    Task<Participant?> GetParticipantByUserIdAsync(string userId);
    Task<List<Participant>> GetParticipantsByDroppedStatusAsync();
    Task<Participant> UpdateParticipantWithCheckinAsync(Checkin checkin, Segment segment);
    Task SendParticipantProfileEmails();
    Task SendParticipantProfileEmail(Guid participantId);
    Task DropParticipantAsync(string participantBib);
}

public class ParticipantService : IParticipantService
{
    private readonly TrackerContext _context;
    private readonly ILeaderService _leaderService;
    private readonly IGraphMailService _graphMailService;

    public ParticipantService(TrackerContext context, ILeaderService leaderService, IGraphMailService graphMailService)
    {
        _context = context;
        _leaderService = leaderService;
        _graphMailService = graphMailService;
    }

    public async Task<List<Participant>> GetParticipantsAsync()
    {
        
        
        return await _context.Participants.Where(x => x.Race!.Active == true).ToListAsync();
        
    }

    public async Task<List<Participant>> GetParticipantsAsync(Guid raceId)
    {
        return await _context.Participants.Where(x => x.RaceId == raceId).ToListAsync();
    }

    public async Task<List<Participant>> GetParticipantsAsync(string raceCode)
    {
        return await _context.Participants.Where(x => x.Race!.Code == raceCode).ToListAsync();
    }

    public async Task<Participant> GetParticipantAsync(Guid participantId, bool includeRace = false)
    {
        if (includeRace)
        {
            return await _context.Participants.Where(x => x.Id == participantId).Include(x => x.Race).SingleAsync();
        }

        return await _context.Participants.Where(x => x.Id == participantId).SingleAsync();
    }

    public async Task<Participant> GetParticipantFromLinkCodeAsync(Guid linkCode) 
    {
        return await _context.Participants.Where(x => x.LinkCode == linkCode).SingleAsync();
    }

    public async Task<Participant> GetParticipantAsync(string id, bool includeRace = false)
    {
        if (Guid.TryParse(id, out var participantId))
        {
            return await GetParticipantAsync(participantId, includeRace);
        }

        // TODO: Clean this up
        if (includeRace)
        {
            return await _context.Participants.Where(x => x.Bib == id).Include(x => x.Race).SingleAsync();
        }

        return await _context.Participants.Where(x => x.Bib == id).SingleAsync();
    }

    public async Task<Participant> GetParticipantWithCheckinsAsync(Guid participantId, bool includeRace = false)
    {
        var query = _context.Participants.Where(x => x.Id == participantId);

        if (includeRace)
        {
            query = query.Include(x => x.Race);
        }

        return await query.Include(x => x.Checkins).AsNoTracking().SingleAsync();
    }

    public async Task<List<Participant>> GetParticipantsByDroppedStatusAsync()
    {
        return await _context.Participants
            //.Include(x => x.LastCheckin)
            //.Include(x => x.LastSegment)
            .Where(x => x.Race.Active == true && (x.Status == Status.DNF || x.Status == Status.DNS))
            .OrderBy(x => x.Bib)
            //.ThenBy(x => x.LastCheckin.When)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Participant> UpdateParticipantWithCheckinAsync(Checkin checkin, Segment segment)
    {
        var participant = await _context.Participants.Where(x => x.Id == checkin.ParticipantId).FirstAsync();
        //participant.LastCheckin = checkin;
        //participant.LastSegment = checkin.Segment;
        //participant.LastCheckpoint = segment.ToCheckpoint;
        //participant.OverallTime = (int)(checkin.When - checkin.Participant.Race.Start).TotalSeconds;
        //participant.OverallPace = (int)TimeHelpers.CalculatePaceInSeconds(checkin.Participant.Race.Start, checkin.When, segment.TotalDistance);
        
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> AddOrUpdateParticipantAsync(Participant participant)
    {
        Participant _participant = new Participant();

        if (await _context.Participants.AnyAsync(x => x.FirstName == participant.FirstName && x.LastName == participant.LastName && x.City == participant.City && x.Region == participant.Region))
        {
            _participant =  await UpdateParticipantAsync(participant);
        }
        else
        {
            _participant = await AddParticipantAsync(participant);
        }

        return _participant;           
    }

    public async Task<Participant> AddParticipantAsync(Participant participant)
    {
        participant.Id = Guid.NewGuid();
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        await _leaderService.AddLeaderAsync(participant);

        return participant;
    }

    public async Task<Participant> UpdateParticipantAsync(Participant participant)
    {
        var p = await _context.Participants.Where(x => x.FirstName == participant.FirstName && x.LastName == participant.LastName && x.City == participant.City && x.Region == participant.Region).FirstAsync();
        p.Region = participant.Region;
        p.City = participant.City;
        p.Age = participant.Age;
        p.Bib = participant.Bib;
        p.Gender = participant.Gender;
        p.RaceId = participant.RaceId;
        p.Rank = participant.Rank;

        await _context.SaveChangesAsync();
        
        return p;
    }

    public async Task<Participant> SetParticipantStatusAsync(Guid participantId, Status status)
    {
        var participant = await GetParticipantAsync(participantId);
        participant.Status = Status.Finished;
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> LinkParticipantToUserIdAsync(Guid linkCode, string userId, string profileImageUrl = "")
    {
        var participant = await GetParticipantFromLinkCodeAsync(linkCode);
        participant.UserId = userId;
        participant.Linked = true;
        if (!String.IsNullOrEmpty(profileImageUrl))
        {
            participant.PictureUrl = profileImageUrl;
        }
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task UpdateParticipantProfileImageUrlAsync(Guid participantId, string profileImageUrl)
    {
        var participant = await GetParticipantAsync(participantId);
        
        if (!String.IsNullOrEmpty(profileImageUrl))
        {
            participant.PictureUrl = profileImageUrl;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<Participant?> GetParticipantByUserIdAsync(string userId)
    {
        return await _context.Participants.SingleOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task SendParticipantProfileEmails()
    {
        var template = File.ReadAllText("Templates/ParticipantRegistration.html");
        var participants = await GetParticipantsAsync();

        foreach (var participant in participants.Where(x => x.Linked == false))
        {
            var emailBody = template.Replace("$$$ClaimProfileLink$$$", $"https://track.runlovit.com/participants/link?linkCode={participant.LinkCode}");
            emailBody = emailBody.Replace("$$$ParticipantFirstName$$$", participant.FirstName);
            await _graphMailService.SendAsync("dustin@runlovit.com", participant.UltraSignupEmail, participant.FullName, "LOViT Tracking Profile", emailBody);
        }
    }

    public async Task SendParticipantProfileEmail(Guid participantId)
    {
        var template = File.ReadAllText("Templates/ParticipantRegistration.html");
        var participant = await GetParticipantAsync(participantId);
        var emailBody = template.Replace("$$$ClaimProfileLink$$$", $"https://track.runlovit.com/participants/link?linkCode={participant.LinkCode}");
        emailBody = emailBody.Replace("$$$ParticipantFirstName$$$", participant.FirstName);
        await _graphMailService.SendAsync("dustin@runlovit.com", participant.UltraSignupEmail, participant.FullName, "LOViT Tracking Profile", emailBody);
    }

    public async Task DropParticipantAsync(string participantBib)
    {
        var participant = await _context.Participants.Where(x => x.Bib == participantBib).FirstAsync();
        participant.Status = Status.DNF;
        await _context.SaveChangesAsync();
    }
}