using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Pages.Admin.Monitors
{
    public class CreateModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public CreateModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CheckpointId"] = new SelectList(_context.Checkpoints, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Models.Monitor Monitor { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Monitors == null || Monitor == null)
            {
                return Page();
            }

            _context.Monitors.Add(Monitor);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
