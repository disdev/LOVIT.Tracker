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

namespace LOVIT.Tracker.Pages.Admin.Checkpoints
{
    [Authorize(Roles="Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public DetailsModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

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
    }
}
