using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LOVIT.Tracker.Models;

public enum Unit
{
    Miles,
    Kilometers
}

public class Race
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public float Distance { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    [Display(Name = "UltraSignup Registration Link")]
    public string UltraSignupUrl { get; set; } = string.Empty;
    public List<Segment> Segments { get; set; } = new();
    public List<Participant> Participants { get; set; } = new();
    [Display(Name = "Race Event")]
    public Guid RaceEventId { get; set; }
    public RaceEvent? RaceEvent { get; set; }
    [Display(Name = "GeoJSON file")]
    public string GeoJson { get; set; } = string.Empty;
    public bool Active { get; set; }
}