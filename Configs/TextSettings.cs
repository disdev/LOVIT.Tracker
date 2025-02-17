namespace LOVIT.Tracker.Models;

public class TextSettings
{
  public string AccountSid { get; set; } = "";
  public string NotificationSid { get; set; } = "";
  public string AuthKey { get; set; } = "";
  public string AdminPhone { get; set; } = "";
  public string SystemPhone { get; set; } = "";
  public bool Enabled { get; set; }
  public string TextBeltUrl { get; set; } = "";
  public string TextBeltKey { get; set; } = "";
  public string WebhookUrl { get; set; } = "";
}