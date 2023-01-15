using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Checkpoints
{
    public class DeleteModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public DeleteModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Checkpoint Checkpoint { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Checkpoints == null)
            {
                return NotFound();
            }

            var checkpoint = await _context.Checkpoints.FirstOrDefaultAsync(m => m.Id == id);

            if (checkpoint == null)
            {
                return NotFound();
            }
            else 
            {
                Checkpoint = checkpoint;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.Checkpoints == null)
            {
                return NotFound();
            }
            var checkpoint = await _context.Checkpoints.FindAsync(id);

            if (checkpoint != null)
            {
                Checkpoint = checkpoint;
                _context.Checkpoints.Remove(Checkpoint);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
