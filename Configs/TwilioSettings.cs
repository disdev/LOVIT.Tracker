namespace LOVIT.Tracker.Models;

public class TwilioSettings
{
  public string AccountSid { get; set; } = "";
  public string NotificationSid { get; set; } = "";
  public string AuthKey { get; set; } = "";
  public string AdminPhone { get; set; } = "";
  public string SystemPhone { get; set; } = "";
  public bool Enabled { get; set; }
}