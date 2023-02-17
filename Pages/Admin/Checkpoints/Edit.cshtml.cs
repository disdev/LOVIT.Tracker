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
using Microsoft.AspNetCore.Authorization;

namespace LOVIT.Tracker.Pages.Admin.Checkpoints
{
    [Authorize(Roles="Administrator")]
    public class EditModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public EditModel(LOVIT.Tracker.Data.TrackerContext context)
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

            var checkpoint =  await _context.Checkpoints.FirstOrDefaultAsync(m => m.Id == id);
            if (checkpoint == null)
            {
                return NotFound();
            }
            Checkpoint = checkpoint;
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

            _context.Attach(Checkpoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckpointExists(Checkpoint.Id))
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

        private bool CheckpointExists(Guid id)
        {
          return (_context.Checkpoints?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
