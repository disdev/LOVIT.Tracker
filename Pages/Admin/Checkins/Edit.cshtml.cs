using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using LOVIT.Tracker.Services;

namespace LOVIT.Tracker.Pages.Admin.Checkins
{
    [Authorize(Roles="Administrator")]
    public class EditModel : PageModel
    {
        private readonly TrackerContext _context;
        private readonly ICheckinService _checkinService;

        public EditModel(LOVIT.Tracker.Data.TrackerContext context, ICheckinService checkinService)
        {
            _context = context;
            _checkinService = checkinService;
        }

        [BindProperty]
        public Checkin Checkin { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Checkins == null)
            {
                return NotFound();
            }

            var checkin =  await _context.Checkins.FirstOrDefaultAsync(m => m.Id == id);
            if (checkin == null)
            {
                return NotFound();
            }
            Checkin = checkin;
           ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id");
           ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "FullName");
           ViewData["SegmentId"] = new SelectList(_context.Segments, "Id", "Name");

            Checkin.When = Checkin.When.ToLocalTime();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Checkin.When = Checkin.When.ToUniversalTime();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _checkinService.ModifyCheckinAsync(Checkin);

            return RedirectToPage("./Index");
        }

        private bool CheckinExists(Guid id)
        {
          return (_context.Checkins?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
