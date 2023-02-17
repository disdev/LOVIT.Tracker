using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using Microsoft.AspNetCore.Authorization;

namespace LOVIT.Tracker.Pages.Admin.Watchers
{
    [Authorize(Roles="Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public DeleteModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Watcher Watcher { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Watchers == null)
            {
                return NotFound();
            }

            var watcher = await _context.Watchers.FirstOrDefaultAsync(m => m.Id == id);

            if (watcher == null)
            {
                return NotFound();
            }
            else 
            {
                Watcher = watcher;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.Watchers == null)
            {
                return NotFound();
            }
            var watcher = await _context.Watchers.FindAsync(id);

            if (watcher != null)
            {
                Watcher = watcher;
                _context.Watchers.Remove(Watcher);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
