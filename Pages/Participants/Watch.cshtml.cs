using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Participants;

[Authorize]
public class WatchModel : PageModel
{
    private readonly ILogger<WatchModel> _logger;
    private readonly IWatcherService _watcherService;
    private readonly IAuth0Service _auth0Service;

    public WatchModel(ILogger<WatchModel> logger, IAuth0Service auth0Service, IWatcherService watcherService)
    {
        _logger = logger;
        _watcherService = watcherService;
        _auth0Service = auth0Service;
    }

    public async Task<IActionResult> OnGetAsync(Guid participantId)
    {
        var userId = User.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        var user = await _auth0Service.GetUserAsync(userId);
        var phone = user.UserMetadata["PhoneNumber"].ToString();

        if (!String.IsNullOrWhiteSpace(phone))
        {
            await _watcherService.AddWatcherAsync(participantId, userId, phone);
        }

        return RedirectToPage("/participants/watchlist");
    }
}