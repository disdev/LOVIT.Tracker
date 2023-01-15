namespace LOVIT.Tracker.Models;

public class AlertMessage
{
    public Guid Id { get; set; }
    public String Message { get; set; } = string.Empty;
    public String Type { get; set; } = string.Empty;
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
}