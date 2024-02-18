namespace LOVIT.Tracker.Models;

public class Leader
{
    public Guid Id { get; set; }
    public Guid  ParticipantId { get; set; }
    public Participant? Participant { get; set; }
    public Guid? LastCheckpointId { get; set; }
    public Checkpoint? LastCheckpoint { get; set; }
    public Guid? LastSegmentId { get; set; }
    public Segment? LastSegment { get; set; }
    public Guid? LastCheckinId { get; set; }
    public Checkin? LastCheckin { get; set; }
    public uint NextPredictedSegmentTime { get; set; }
    public uint OverallTime { get; set; }
    public uint OverallPace { get; set; }
}
