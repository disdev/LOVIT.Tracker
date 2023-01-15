/*
namespace LOVIT.Tracker.Models;

public class Leaderboard
{
    public Guid Id { get; set; }
    public DateTime Updated { get; set; }
    public Race? Race { get; set; }
    public List<LeaderboardEntry>? LeaderboardEntries { get; set; }
}

public class LeaderboardEntry
{
    public LeaderboardEntry() {}
    public LeaderboardEntry(Participant participant, int place)
    {
        Place = place;
        ParticipantBib = participant!.Bib;
        ParticipantFirstName = participant.FirstName;
        ParticipantLastName = participant.LastName;
        ParticipantRegion = participant.Region;
        ParticipantAge = participant.Age;
        ParticipantGender = participant.Gender;
        ParticipantId = participant.Id;
        LastCheckpointId = participant.LastCheckpoint.Id;
        LastCheckpointName = participant.LastCheckpoint.Name;
        LastSegmentId = participant.LastSegment.Id;
        LastSegmentName = participant.LastSegment.Name;
        LastSegmentTotalDistance = participant.LastSegment.TotalDistance;
        LastSeen = participant.LastCheckin.When;
        OverallTime = participant.OverallTime;
        OverallPace = participant.OverallPace;
    }

    public int Place { get; set; }
    public string? ParticipantBib { get; set; }
    public string? ParticipantFirstName { get; set; }
    public string? ParticipantLastName { get; set; }
    public string? ParticipantRegion { get; set; }
    public string? ParticipantAge { get; set; }
    public Gender ParticipantGender { get; set; }
    public Guid? ParticipantId { get; set; }
    public Guid? LastCheckpointId { get; set; }
    public string? LastCheckpointName { get; set; }
    public Guid? LastSegmentId { get; set; }
    public string? LastSegmentName { get; set; }
    public double? LastSegmentTotalDistance { get; set; }
    public DateTime LastSeen { get; set; }
    public int OverallTime { get; set; }
    public int OverallPace { get; set; }
}
*/