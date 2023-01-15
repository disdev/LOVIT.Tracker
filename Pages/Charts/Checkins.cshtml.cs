using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Charts
{
    public class CheckinsModel : PageModel
    {
        private readonly IRaceService _raceService;

        public CheckinsModel(IRaceService raceService)
        {
            _raceService = raceService;
        }

        public async Task OnGet()
        {
        }
    }
}
