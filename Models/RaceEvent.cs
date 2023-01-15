using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LOVIT.Tracker.Models;

public class RaceEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public List<Race>? Races { get; set; }
    public bool Current { get; set; }
}