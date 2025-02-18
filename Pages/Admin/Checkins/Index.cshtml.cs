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

namespace LOVIT.Tracker.Pages.Admin.Checkins
{
    [Authorize(Roles="Administrator")]
    public class IndexModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public IndexModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        public IList<Checkin> Checkin { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Checkins != null)
            {
                Checkin = await _context.Checkins
                .Include(c => c.Message)
                .Include(c => c.Participant)
                .Include(c => c.Segment)
                .OrderBy(c => c.When).ToListAsync();

                foreach (var checkin in Checkin)
                {
                    checkin.When = checkin.When.ToLocalTime();
                }
            }
        }
    }
}
