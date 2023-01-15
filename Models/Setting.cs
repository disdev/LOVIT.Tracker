using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LOVIT.Tracker.Models;

public class Setting
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}