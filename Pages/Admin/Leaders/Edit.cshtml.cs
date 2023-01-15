using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Leaders
{
    public class EditModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public EditModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Leader Leader { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Leaders == null)
            {
                return NotFound();
            }

            var leader =  await _context.Leaders.FirstOrDefaultAsync(m => m.Id == id);
            if (leader == null)
            {
                return NotFound();
            }
            Leader = leader;
           ViewData["LastCheckinId"] = new SelectList(_context.Checkins, "Id", "Id");
           ViewData["LastCheckpointId"] = new SelectList(_context.Checkpoints, "Id", "Id");
           ViewData["LastSegmentId"] = new SelectList(_context.Segments, "Id", "Id");
           ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Leader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaderExists(Leader.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LeaderExists(Guid id)
        {
          return (_context.Leaders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
