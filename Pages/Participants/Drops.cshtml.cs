using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Participants;

public class DropsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ILeaderService _leaderService;

    public DropsModel(ILogger<IndexModel> logger, ILeaderService leaderService)
    {
        _leaderService = leaderService;
    }

    public List<Leader> Leaders { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Leaders = await _leaderService.GetLeadersByDroppedStatusAsync();

        return Page();
    }
}
