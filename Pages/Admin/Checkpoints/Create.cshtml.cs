using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using Microsoft.AspNetCore.Authorization;

namespace LOVIT.Tracker.Pages.Admin.Checkpoints
{
    [Authorize(Roles="Administrator")]
public class CreateModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;

        public CreateModel(LOVIT.Tracker.Data.TrackerContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Checkpoint Checkpoint { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Checkpoints == null || Checkpoint == null)
            {
                return Page();
            }

            _context.Checkpoints.Add(Checkpoint);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
