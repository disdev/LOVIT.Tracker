using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LOVIT.Tracker.Models;

public class Message
{
    public Guid Id { get; set; }
    public string From { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string FromState { get; set; } = string.Empty;
    public string FromCountry { get; set; } = string.Empty;
    public string FromZip { get; set; } = string.Empty;
    public DateTime Received { get; set; }
}