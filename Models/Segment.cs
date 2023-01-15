using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LOVIT.Tracker.Models;

public class Segment
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Race? Race { get; set; }
    [Display(Name = "Race")]
    public Guid RaceId { get; set; }
    public int Order { get; set; }
    public string GeoJson { get; set; } = string.Empty;
    public Guid? FromCheckpointId { get; set; }
    public Checkpoint? FromCheckpoint { get; set; }
    public Guid? ToCheckpointId { get; set; }
    public Checkpoint? ToCheckpoint { get; set; }
    public Double Distance { get; set; }
    public Double TotalDistance { get; set; }
}