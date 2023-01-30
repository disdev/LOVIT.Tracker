using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Utilities;

namespace LOVIT.Tracker.Pages.Admin.Checkins
{
    public class DeleteModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public DeleteModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Checkin Checkin { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Checkins == null)
            {
                return NotFound();
            }

            var checkin = await _context.Checkins.FirstOrDefaultAsync(m => m.Id == id);

            if (checkin == null)
            {
                return NotFound();
            }
            else 
            {
                Checkin = checkin;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.Checkins == null)
            {
                return NotFound();
            }
            var checkin = await _context.Checkins.FindAsync(id);
            var participant = await _context.Participants.FindAsync(checkin.ParticipantId);
            var race = await _context.Races.SingleAsync(x => x.Id == participant.RaceId);
            var participantCheckins = await _context.Checkins.Where(x => x.ParticipantId == participant.Id).OrderBy(x => x.When).ToListAsync();
            var leader = await _context.Leaders.Where(x => x.ParticipantId == participant.Id).SingleAsync();

            if (checkin != null)
            {
                if (participantCheckins.Last().Id == checkin.Id)
                {
                    var lastCheckin = participantCheckins.TakeLast(2).First();
                    var lastSegment = await _context.Segments.SingleAsync(x => x.Id == lastCheckin.SegmentId);
                    leader.LastCheckinId = lastCheckin.Id;
                    leader.OverallTime = (uint)(lastCheckin.When - race.Start).TotalSeconds;
                    leader.OverallPace = (uint)TimeHelpers.CalculatePaceInSeconds((long)leader.OverallTime, lastSegment.TotalDistance);
                    leader.LastCheckpointId = lastSegment.ToCheckpointId;
                    leader.LastSegmentId = lastSegment.Id;
                }
                Checkin = checkin;
                _context.Checkins.Remove(Checkin);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
