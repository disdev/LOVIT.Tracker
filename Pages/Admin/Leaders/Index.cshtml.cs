using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Leaders
{
    public class IndexModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public IndexModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        public IList<Leader> Leader { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Leaders != null)
            {
                Leader = await _context.Leaders
                .Include(l => l.LastCheckin)
                .Include(l => l.LastCheckpoint)
                .Include(l => l.LastSegment)
                .Include(l => l.Participant)
                .ThenInclude(p => p.Race)
                .ToListAsync();
            }
        }
    }
}
