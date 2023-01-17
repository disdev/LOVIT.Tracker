using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Data;

public class TrackerContext : DbContext
{
    public TrackerContext (DbContextOptions<TrackerContext> options) : base(options)
    {
    }

    public DbSet<RaceEvent> RaceEvents { get; set; } = default!;
    public DbSet<Race> Races { get; set; } = default!;
    public DbSet<Checkpoint> Checkpoints { get; set; } = default!;
    public DbSet<Segment> Segments { get; set; } = default!;
    public DbSet<Participant> Participants { get; set; } = default!;
    public DbSet<LOVIT.Tracker.Models.Monitor> Monitors { get; set; } = default!;
    public DbSet<Checkin> Checkins { get; set; } = default!;
    public DbSet<Watcher> Watchers { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<Setting> Settings { get; set; } = default!;
    public DbSet<AlertMessage> AlertMessages { get; set; } = default!;
    public DbSet<Leader> Leaders { get; set; } = default!;
}