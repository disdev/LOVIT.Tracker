using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Checkins
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
        ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id");
        ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "Id");
        ViewData["SegmentId"] = new SelectList(_context.Segments, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Checkin Checkin { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Checkins == null || Checkin == null)
            {
                return Page();
            }

            _context.Checkins.Add(Checkin);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
