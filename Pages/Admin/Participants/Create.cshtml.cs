using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Authorization;

namespace LOVIT.Tracker.Pages.Admin.Participants
{
    [Authorize(Roles="Administrator")]
public class CreateModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;
        private readonly IParticipantService _participantService;

        public CreateModel(LOVIT.Tracker.Data.TrackerContext context, IParticipantService participantService)
        {
            _context = context;
            _participantService = participantService;
        }

        public IActionResult OnGet()
        {
        ViewData["RaceId"] = new SelectList(_context.Races, "Id", "Code");
            return Page();
        }

        [BindProperty]
        public Participant Participant { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var participant = await _participantService.AddOrUpdateParticipantAsync(Participant);

            return RedirectToPage("./Index");
        }
    }
}
