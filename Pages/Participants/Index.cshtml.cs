using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LOVIT.Tracker.Services;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Utilities;

namespace LOVIT.Tracker.Pages.Participants;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IParticipantService _participantService;
    private readonly IRaceService _raceService;
    private readonly IWatcherService _watcherService;
    private readonly IAuth0Service _auth0Service;

    public IndexModel(ILogger<IndexModel> logger, IParticipantService participantService, IRaceService raceService, IWatcherService watcherService, IAuth0Service auth0Service)
    {
        _logger = logger;
        _participantService = participantService;
        _watcherService = watcherService;
        _raceService = raceService;
        _auth0Service = auth0Service;
    }

    public Race Race { get; set; } = new();
    public Participant Participant { get; set; } = new();
    
    public bool PhoneNumberSet { get; set; } = false;
    public bool ShowNotifyOption { get; set; } = true;
    public bool IsAuthenticated { get; set; } = false;

    public async Task OnGet(string id)
    {
        Participant = await _participantService.GetParticipantAsync(id, true);
        Race = await _raceService.GetRaceAsync(Participant.RaceId);
        //await LoadData(id);
        await SetNotificationOptions();
        ViewData["Title"] = Participant.FullName;
    }

    private async Task SetNotificationOptions()
    {
        IsAuthenticated = User.Identity.IsAuthenticated;
        var userId = User.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        if (User.Identity.IsAuthenticated && !String.IsNullOrEmpty(await _auth0Service.GetUserPhoneNumber(userId)))
        {
            PhoneNumberSet = true;

            if (await _watcherService.WatcherExists(Participant.Id, userId) == true)
            {
                ShowNotifyOption = false;
            }
        }
    }
}

