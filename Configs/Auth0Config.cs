namespace LOVIT.Tracker.Models;

public class Auth0Config
{
    public string AppClientId { get; set; } = "";
    public string AppClientSecret { get; set; } = "";
    public string ManagementClientId { get; set; } = "";
    public string ManagementClientSecret { get; set; } = "";
    public string Domain { get; set; } = "";
    public string Audience { get; set; } = "";
    public string ManagementAudience { get; set; } = "";
}