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

namespace LOVIT.Tracker.Pages.Admin.AlertMessages
{
    public class EditModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public EditModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AlertMessage AlertMessage { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.AlertMessages == null)
            {
                return NotFound();
            }

            var alertmessage =  await _context.AlertMessages.FirstOrDefaultAsync(m => m.Id == id);
            if (alertmessage == null)
            {
                return NotFound();
            }
            AlertMessage = alertmessage;
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

            _context.Attach(AlertMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlertMessageExists(AlertMessage.Id))
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

        private bool AlertMessageExists(Guid id)
        {
          return (_context.AlertMessages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
