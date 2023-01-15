using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.RaceEvents
{
    public class DeleteModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public DeleteModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
      public RaceEvent RaceEvent { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.RaceEvents == null)
            {
                return NotFound();
            }

            var raceevent = await _context.RaceEvents.FirstOrDefaultAsync(m => m.Id == id);

            if (raceevent == null)
            {
                return NotFound();
            }
            else 
            {
                RaceEvent = raceevent;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.RaceEvents == null)
            {
                return NotFound();
            }
            var raceevent = await _context.RaceEvents.FindAsync(id);

            if (raceevent != null)
            {
                RaceEvent = raceevent;
                _context.RaceEvents.Remove(RaceEvent);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
