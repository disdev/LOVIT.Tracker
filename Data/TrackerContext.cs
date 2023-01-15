using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Data;

public class TrackerContext : DbContext
{
    public TrackerContext (DbContextOptions<TrackerContext> options)
        : base(options)
    {
    }

    public DbSet<RaceEvent> RaceEvents { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Checkpoint> Checkpoints { get; set; }
    public DbSet<Segment> Segments { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<LOVIT.Tracker.Models.Monitor> Monitors { get; set; }
    public DbSet<Checkin> Checkins { get; set; }
    public DbSet<Watcher> Watchers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<AlertMessage> AlertMessages { get; set; }
    public DbSet<Leader> Leaders { get; set; }
}