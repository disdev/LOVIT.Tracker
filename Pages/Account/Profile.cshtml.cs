using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using LOVIT.Tracker.Services;
using LOVIT.Tracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAuth0Service _auth0Service;
    private readonly IParticipantService _participantService;
    private readonly ITextService _textService;

    public User Auth0User { get; set; } = new();
    public bool AlertVisible { get; set; } = false;
    public string AlertType { get; set; } = "info";
    public string AlertMessage { get; set; } = "";
    public bool IsParticipant { get; set; }
    public Participant Participant { get; set; } = new();

    [BindProperty]
    public UserProfileViewModel UserProfile { get; set; } = new();

    public ProfileModel(ILogger<IndexModel> logger, IAuth0Service auth0Service, IParticipantService participantService, ITextService textService)
    {
        _logger = logger;
        _auth0Service = auth0Service;
        _participantService = participantService;
        _textService = textService;
    }

    public async Task OnGetAsync()
    {
        var userId = User.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        Auth0User = await _auth0Service.GetUserAsync(userId);

        UpdateFields();

        Participant = await _participantService.GetParticipantByUserIdAsync(userId);
        if (Participant != null)
        {
            IsParticipant = true;
            await _participantService.UpdateParticipantProfileImageUrlAsync(Participant.Id, Auth0User.Picture);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("UserProfile.EmailAddress");
        ModelState.Remove("UserProfile.FirstName");
        ModelState.Remove("UserProfile.LastName");
        ModelState.Remove("UserProfile.PhoneNumber");

        var phoneNumber = _textService.CheckPhoneNumber(UserProfile.PhoneNumber);

        var userId = User.Claims!.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        
        if (String.IsNullOrEmpty(phoneNumber) && UserProfile.PhoneNumber != phoneNumber)
        {
            AlertVisible = true;
            AlertMessage = "Whoops! Looks like the phone number you entered is invalid.";
            AlertType = "warning";
        }
        else
        {
            Auth0User = await _auth0Service.UpdateUserAsync(userId, UserProfile.FirstName, UserProfile.LastName, phoneNumber);
            AlertVisible = true;
            AlertMessage = "Your profile has been updated.";
        }
        
        UpdateFields();

        return Page();
    }

    private void UpdateFields()
    {
        UserProfile.FirstName = Auth0User.FirstName;
        UserProfile.LastName = Auth0User.LastName;
        if (Auth0User.UserMetadata != null)
        {
            UserProfile.PhoneNumber = Auth0User.UserMetadata["PhoneNumber"].Value;
        }
        UserProfile.EmailAddress = Auth0User.Email;
    }

    public class UserProfileViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "";
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = "";
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = "";
    }
}
