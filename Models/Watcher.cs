using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LOVIT.Tracker.Models;

public class Watcher
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid ParticipantId { get; set; }
    public Participant? Participant { get; set; }
    public bool Disabled { get; set; }
    public string UserId { get; set; } = string.Empty;
}