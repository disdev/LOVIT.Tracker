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

namespace LOVIT.Tracker.Pages.Admin.Races
{
    [Authorize(Roles="Administrator")]
    public class IndexModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public IndexModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        public IList<Race> Races { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Races != null)
            {
                Races = await _context.Races.Include(r => r.RaceEvent).ToListAsync();
                foreach (var race in Races)
                {
                    race.Start = race.Start.ToLocalTime();
                    race.End = race.End.ToLocalTime();
                }
                
            }
        }
    }
}
