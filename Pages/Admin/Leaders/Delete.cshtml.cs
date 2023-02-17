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

namespace LOVIT.Tracker.Pages.Admin.Leaders
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
      public Leader Leader { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Leaders == null)
            {
                return NotFound();
            }

            var leader = await _context.Leaders.FirstOrDefaultAsync(m => m.Id == id);

            if (leader == null)
            {
                return NotFound();
            }
            else 
            {
                Leader = leader;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.Leaders == null)
            {
                return NotFound();
            }
            var leader = await _context.Leaders.FindAsync(id);

            if (leader != null)
            {
                Leader = leader;
                _context.Leaders.Remove(Leader);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
