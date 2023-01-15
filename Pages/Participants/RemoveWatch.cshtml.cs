using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Participants;

[Authorize]
public class RemoveWatchModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IWatcherService _watcherService;
    private readonly IAuth0Service _auth0Service;
    private readonly IParticipantService _participantService;

    public RemoveWatchModel(ILogger<IndexModel> logger, IAuth0Service auth0Service, IWatcherService watcherService, IParticipantService participantService)
    {
        _logger = logger;
        _watcherService = watcherService;
        _auth0Service = auth0Service;
        _participantService = participantService;
    }

    public Participant Participant { get; set; }

    public async Task OnGetAsync(Guid participantId)
    {
        var userId = User.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        Participant = await _participantService.GetParticipantAsync(participantId);
    }

    public async Task<IActionResult> OnPost(Guid participantId)
    {
        var userId = User.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        await _watcherService.RemoveWatcher(participantId, userId);
        return RedirectToPage("/participants/watchlist");
    }
}