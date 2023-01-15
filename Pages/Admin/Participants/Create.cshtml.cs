using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Participants
{
    public class CreateModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public CreateModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
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
          if (!ModelState.IsValid || _context.Participants == null || Participant == null)
            {
                return Page();
            }

            _context.Participants.Add(Participant);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
