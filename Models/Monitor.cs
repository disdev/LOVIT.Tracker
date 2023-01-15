using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace LOVIT.Tracker.Models;

public class Monitor
{
    public Guid Id { get; set; }
    public Guid CheckpointId { get; set; }
    public Checkpoint? Checkpoint { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool Active { get; set; }
}
