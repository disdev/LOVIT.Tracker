using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Participants;

[Authorize]
public class LinkModel : PageModel
{
    private readonly ILogger<LinkModel> _logger;
    private readonly IParticipantService _participantService;
    private readonly IAuth0Service _auth0Service;

    public LinkModel(ILogger<LinkModel> logger, IParticipantService participantService, IAuth0Service auth0Service)
    {
        _logger = logger;
        _participantService = participantService;
        _auth0Service = auth0Service;
    }

    public Participant Participant { get; set; } = new();

    public async Task OnGetAsync(Guid linkCode)
    {
        var userId = User.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        var user = await _auth0Service.GetUserAsync(userId);
        Participant = await _participantService.LinkParticipantToUserIdAsync(linkCode, userId, user.Picture);
    }
}