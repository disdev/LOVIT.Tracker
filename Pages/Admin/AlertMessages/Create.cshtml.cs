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

namespace LOVIT.Tracker.Pages.Admin.AlertMessages
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
        public AlertMessage AlertMessage { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.AlertMessages == null || AlertMessage == null)
            {
                return Page();
            }

            _context.AlertMessages.Add(AlertMessage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
