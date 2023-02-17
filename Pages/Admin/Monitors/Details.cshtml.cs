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

namespace LOVIT.Tracker.Pages.Admin.Monitors
{
    [Authorize(Roles="Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public DetailsModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

      public Models.Monitor Monitor { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Monitors == null)
            {
                return NotFound();
            }

            var monitor = await _context.Monitors.FirstOrDefaultAsync(m => m.Id == id);
            if (monitor == null)
            {
                return NotFound();
            }
            else 
            {
                Monitor = monitor;
            }
            return Page();
        }
    }
}
