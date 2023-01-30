using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Monitors
{
    public class IndexModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public IndexModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        public IList<Models.Monitor> Monitor { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Monitors != null)
            {
                Monitor = await _context.Monitors
                .Include(m => m.Checkpoint)
                .OrderBy(m => m.Checkpoint.Number).ToListAsync();
            }
        }
    }
}
