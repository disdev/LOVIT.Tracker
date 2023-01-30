using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IParticipantService _participantService;
    private readonly ILeaderService _leaderService;
    private readonly IRaceService _raceService;

    public List<Leader> Leaderboard = new();
    public List<Race> Races = new();

    public IndexModel(ILogger<IndexModel> logger, IParticipantService participantService, ILeaderService leaderService, IRaceService raceService)
    {
        _logger = logger;
        _participantService = participantService;
        _leaderService = leaderService;
        _raceService = raceService;
    }

    public async Task<IActionResult> OnGet()
    {
        //Leaderboard = await _leaderService.GetLeadersAsync();
        Races = await _raceService.GetRacesAsync();

        return Page();
    }
}
