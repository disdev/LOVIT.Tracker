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
using LOVIT.Tracker.Services;

namespace LOVIT.Tracker.Pages.Admin.Monitors
{
    [Authorize(Roles="Administrator")]
public class CreateModel : PageModel
    {
        private readonly LOVIT.Tracker.Data.TrackerContext _context;
        private readonly ITextService _textService;

        public CreateModel(LOVIT.Tracker.Data.TrackerContext context, ITextService textService)
        {
            _context = context;
            _textService = textService;
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

            var checkpoint = await _context.Checkpoints.FindAsync(Monitor.CheckpointId);

            await _textService.SendMessageAsync(Monitor.PhoneNumber, $"You're set up as a monitor for {checkpoint.Name}.");
            await _textService.SendAdminMessageAsync($"A number has been set up as a monitor for {Monitor.Checkpoint.Name}.");

            return RedirectToPage("./Index");
        }
    }
}
