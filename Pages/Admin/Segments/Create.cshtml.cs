using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Segments
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
        ViewData["FromCheckpointId"] = new SelectList(_context.Checkpoints, "Id", "Name");
        ViewData["RaceId"] = new SelectList(_context.Races, "Id", "Name");
        ViewData["ToCheckpointId"] = new SelectList(_context.Checkpoints, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Segment Segment { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Segments == null || Segment == null)
            {
                return Page();
            }

            _context.Segments.Add(Segment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
