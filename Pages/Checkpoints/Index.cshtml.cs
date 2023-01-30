using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LOVIT.Tracker.Services;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Checkpoints;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRaceService _raceService;
    private readonly IParticipantService _participantService;
    private readonly ISegmentService _segmentService;
    private readonly ICheckinService _checkinService;
    private readonly ILeaderService _leaderService;
    private readonly ICheckpointService _checkpointService;

    public IndexModel(ILogger<IndexModel> logger, IParticipantService participantService, IRaceService raceService, ISegmentService segmentService, ICheckinService checkinService, ILeaderService leaderService, ICheckpointService checkpointService)
    {
        _logger = logger;
        _raceService = raceService;
        _participantService = participantService;
        _segmentService = segmentService;
        _checkinService = checkinService;
        _leaderService = leaderService;
        _checkpointService = checkpointService;
    }

    public List<Race> Races { get; set; } = new();
    public List<Segment> Segments { get; set; } = new();
    public List<Leader> Leaders { get; set; } = new();
    public List<Checkin> Checkins { get; set; } = new();
    public Checkpoint Checkpoint { get; set; } = new();

    public async Task OnGetAsync(Guid id)
    {
        await LoadData(id);
    }

    public async Task LoadData(Guid checkpointId)
    {
        Checkpoint = await _checkpointService.GetCheckpointAsync(checkpointId);
        Races = await _raceService.GetRacesFromCheckpointAsync(checkpointId);
        Segments = await _segmentService.GetSegmentsAsync();
        Leaders = await _leaderService.GetLeadersAsync();
        Checkins = await _checkinService.GetCheckinsForCheckpointAsync(checkpointId);

        ViewData["Title"] = Checkpoint.Name;
    }
}

