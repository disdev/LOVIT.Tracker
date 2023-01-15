using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Participants;

public class DropsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IParticipantService _participantService;
    private readonly ISegmentService _segmentService;
    private readonly ICheckinService _checkinService;
    private readonly ILeaderService _leaderService;

    public DropsModel(ILogger<IndexModel> logger, IParticipantService participantService, ISegmentService segmentService, ICheckinService checkinService, ILeaderService leaderService)
    {
        _logger = logger;
        _participantService = participantService;
        _segmentService = segmentService;
        _checkinService = checkinService;
        _leaderService = leaderService;
    }

    public List<Leader> Leaders { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        Leaders = await _leaderService.GetLeadersByDroppedStatusAsync();

        return Page();
    }
}
