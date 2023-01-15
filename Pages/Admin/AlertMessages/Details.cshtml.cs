using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.AlertMessages
{
    public class DetailsModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public DetailsModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

      public AlertMessage AlertMessage { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.AlertMessages == null)
            {
                return NotFound();
            }

            var alertmessage = await _context.AlertMessages.FirstOrDefaultAsync(m => m.Id == id);
            if (alertmessage == null)
            {
                return NotFound();
            }
            else 
            {
                AlertMessage = alertmessage;
            }
            return Page();
        }
    }
}
