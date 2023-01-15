using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Leaders
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
        ViewData["LastCheckinId"] = new SelectList(_context.Checkins, "Id", "Id");
        ViewData["LastCheckpointId"] = new SelectList(_context.Checkpoints, "Id", "Id");
        ViewData["LastSegmentId"] = new SelectList(_context.Segments, "Id", "Id");
        ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Leader Leader { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Leaders == null || Leader == null)
            {
                return Page();
            }

            _context.Leaders.Add(Leader);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
