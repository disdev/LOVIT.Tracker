using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOVIT.Tracker.Pages.Segments
{
    public class IndexModel : PageModel
    {
        private readonly ISegmentService _segmentService;
        private readonly IRaceService _raceService;
        private readonly ILeaderService _leaderService;
        private readonly ICheckinService _checkinService;

        public IndexModel(ISegmentService segmentService, IRaceService raceService, ILeaderService leaderService, ICheckinService checkinService)
        {
            _segmentService = segmentService;
            _raceService = raceService;
            _leaderService = leaderService;
            _checkinService = checkinService;
        }

        public Segment Segment = new();
        public Race Race = new();
        public List<Leader> Leaders = new();
        public List<Checkin> Checkins = new();
        
        public async Task<ActionResult> OnGet(Guid id)
        {
            Segment = await _segmentService.GetSegmentAsync(id);
            Race = await _raceService.GetRaceAsync(Segment.RaceId);
            Leaders = await _leaderService.GetLeadersByRaceIdAsync(Race.Id);
            Checkins = await _checkinService.GetCheckinsForSegmentAsync(Segment.Id);

            ViewData["Title"] = Segment.Name;
            return Page();
        }
    }
}
