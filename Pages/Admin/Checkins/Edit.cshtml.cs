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

namespace LOVIT.Tracker.Pages.Admin.Checkins
{
    [Authorize(Roles="Administrator")]
    public class EditModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public EditModel(LOVIT.Tracker.Data.TrackerContext context)
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

            var checkin =  await _context.Checkins.FirstOrDefaultAsync(m => m.Id == id);
            if (checkin == null)
            {
                return NotFound();
            }
            Checkin = checkin;
           ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id");
           ViewData["ParticipantId"] = new SelectList(_context.Participants, "Id", "FullName");
           ViewData["SegmentId"] = new SelectList(_context.Segments, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Checkin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckinExists(Checkin.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CheckinExists(Guid id)
        {
          return (_context.Checkins?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
