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
    public class IndexModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public IndexModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        public IList<Checkpoint> Checkpoint { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Checkpoints != null)
            {
                Checkpoint = await _context.Checkpoints.OrderBy(x => x.Number).ToListAsync();
            }
        }
    }
}
