using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LOVIT.Tracker.Models;

public class Checkpoint
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GeoJson { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Code { get; set; } = string.Empty;
    public List<Monitor> Monitors { get; set; } = new();
}