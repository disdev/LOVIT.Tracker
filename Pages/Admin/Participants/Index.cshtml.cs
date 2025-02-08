using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Authorization;

namespace LOVIT.Tracker.Pages.Admin.Participants
{
    [Authorize(Roles="Administrator")]
    public class IndexModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;
        private readonly IRaceService _raceService;

        public IndexModel(LOVIT.Tracker.Data.TrackerContext context, IRaceService raceService)
        {
            _context = context;
            _raceService = raceService;
        }

        public IList<Participant> Participant { get;set; } = default!;

        public async Task OnGetAsync()
        {
            await _raceService.SyncParticipantsWithUltraSignup();

            if (_context.Participants != null)
            {
                Participant = await _context.Participants
                .Include(p => p.Race)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName).ToListAsync();
            }
        }
    }
}
