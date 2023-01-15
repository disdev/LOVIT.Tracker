namespace LOVIT.Tracker.Models;

public class SegmentPredictionModelInput
{
    public string FullName { get; set; }

    public string Age { get; set; }

    public float Gender { get; set; }

    public float Rank { get; set; }

    public string RaceCode { get; set; }

    public float SegmentOrder { get; set; }

    public float SegmentDistance { get; set; }

    public float TotalDistance { get; set; }

    public float LastTotalElapsed { get; set; }

    public float SegmentElapsed { get; set; }
}

public class SegmentPredictionModelOuput
{
    public float SegmentElapsed { get; set; }
}
