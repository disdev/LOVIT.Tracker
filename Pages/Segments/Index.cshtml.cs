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
        private readonly IParticipantService _participantService;
        private readonly ICheckinService _checkinService;

        public IndexModel(ISegmentService segmentService, IRaceService raceService, IParticipantService participantService, ICheckinService checkinService)
        {
            _segmentService = segmentService;
            _raceService = raceService;
            _participantService = participantService;
            _checkinService = checkinService;
        }

        public Segment Segment = new();
        public Race Race = new();
        public List<Participant> Participants = new();
        public List<Checkin> Checkins = new();
        
        public async void OnGet(Guid id)
        {
            Segment = await _segmentService.GetSegmentAsync(id);
            Race = await _raceService.GetRaceAsync(Segment.RaceId);
            Participants = await _participantService.GetParticipantsAsync(Race.Id);
            Checkins = await _checkinService.GetCheckinsForSegmentAsync(Segment.Id);
        }
    }
}
