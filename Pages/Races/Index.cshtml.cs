using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Races;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRaceService _raceService;
    private readonly IParticipantService _participantService;
    private readonly ISegmentService _segmentService;
    private readonly ICheckinService _checkinService;
    private readonly ILeaderService _leaderService;

    public Race Race { get; set; } = new();
    public List<Segment> Segments { get; set; } = new();
    public List<Leader> Leaders { get; set; } = new();
    public List<Checkin> Checkins { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, IParticipantService participantService, ILeaderService leaderService, IRaceService raceService, ISegmentService segmentService, ICheckinService checkinService)
    {
        _logger = logger;
        _participantService = participantService;
        _leaderService = leaderService;
        _raceService = raceService;
        _segmentService = segmentService;
        _checkinService = checkinService;
    }

    public async Task OnGetAsync(string id)
    {
        Race = await _raceService.GetRaceAsync(id);
        Leaders = await _leaderService.GetLeadersByRaceIdAsync(Race.Id);
        Segments = await _segmentService.GetSegmentsAsync(Race.Id);
        Checkins = await _checkinService.GetCheckinsAsync(Race.Id);
    }
}