using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Maps
{
    public class IndexModel : PageModel
    {
        private readonly ISegmentService _segmentService;
        private readonly IRaceService _raceService;

        public IndexModel(ISegmentService segmentService, IRaceService raceService)
        {
            _segmentService = segmentService;
            _raceService = raceService;
        }

        public List<Race> Races = new();
        public List<Segment> Segments = new();

        public async Task OnGet()
        {
            Races = await _raceService.GetRacesAsync();
            Segments = await _segmentService.GetSegmentsAsync();
        }
    }
}
