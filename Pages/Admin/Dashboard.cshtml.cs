using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Services;

namespace LOVIT.Tracker.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IRaceService _raceService;
        private readonly ISegmentService _segmentService;
        private readonly IParticipantService _participantService;
        private readonly ILeaderService _leaderService;
        private readonly IPredictionService _predictionService;

        public DashboardModel(IRaceService raceService, ISegmentService segmentService, IParticipantService participantService, ILeaderService leaderService, IPredictionService predictionService)
        {
            _raceService = raceService;
            _segmentService = segmentService;
            _participantService = participantService;
            _leaderService = leaderService;
            _predictionService = predictionService;
        }

        public IList<Leader> Leaders { get;set; } = default!;
        public IList<Segment> Segments { get; set; } = default!;
        public IList<Race> Races { get; set; } = default!;
        public List<IncomingViewModel> IncomingParticipants { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Races = await _raceService.GetRacesAsync();
            Leaders = await _leaderService.GetLeadersAsync();
            Segments = await _segmentService.GetSegmentsAsync();
            
            Segments.OrderBy(x => x.Race).ThenBy(x => x.Order);
            
            var predictionsToMake = new List<SegmentPredictionInput>();

            IncomingParticipants = new List<IncomingViewModel>();

            foreach (var leader in Leaders)
            {
                var race = Races.Where(x => x.Id == leader.Participant.RaceId).First();
                var lastSegment = Segments.Where(x => x.RaceId == leader.Participant.RaceId).Last();
                var firstSegment = Segments.Where(x => x.RaceId == leader.Participant.RaceId).First();
                Segment nextSegment = new Segment();

                if (leader.Participant.Status == Status.Registered)
                {
                    nextSegment = firstSegment;
                } 
                else if (leader.Participant.Status == Status.Started)
                {
                    nextSegment = GetNextSegment(leader.LastSegment, Races.First(x => x.Id == leader.Participant.RaceId));
                }
                
                if (nextSegment != null)
                {
                    predictionsToMake.Add(new SegmentPredictionInput()
                    {
                        Leader = leader,
                        Segment = nextSegment,
                        RaceCode = race.Code,
                        LastTotalElapsed = leader.OverallTime
                    });
                }

                IncomingParticipants.Add(new IncomingViewModel() {
                    Leader = leader,
                    RaceCode = race.Code,
                    NextSegment = nextSegment
                });
            }

            var predictions = await _predictionService.GetEstimatesAsync(predictionsToMake);
            foreach (var prediction in predictions)
            {
                var incomingParticipant = IncomingParticipants.Where(x => x.Leader.Participant.FullName == prediction.FullName).First();
                var race = Races.Where(x => x.Id == incomingParticipant.Leader.Participant.RaceId).First();
                incomingParticipant.Prediction = prediction;
                incomingParticipant.DueDate = race.Start.AddSeconds(incomingParticipant.Leader.OverallTime).AddSeconds(prediction.SegmentElapsed);
                var gap = (DateTime.UtcNow - incomingParticipant.DueDate).TotalMinutes;
                
                if (gap >= 30)
                {
                    incomingParticipant.RowClass = "table-danger";
                }
                else if (gap >= 15)
                {
                    incomingParticipant.RowClass = "table-warning";
                }
                else if (gap >= 0)
                {
                    incomingParticipant.RowClass = "table-info";
                }

            }
        }

        private Segment GetNextSegment(Segment currentSegment, Race race)
        {
            if (currentSegment == null) {
                return Segments.Where(x => x.RaceId == race.Id).First();
            }
            return Segments.Where(x => x.RaceId == race.Id).SkipWhile(x => x.Id != currentSegment.Id).Skip(1).FirstOrDefault();
        }

        public class IncomingViewModel
        {
            public Leader Leader { get; set; }
            public string RaceCode { get; set; }
            public Segment NextSegment { get; set; }
            public SegmentPredictionModelInput Prediction { get; set; }
            public DateTime DueDate { get; set; }
            public string RowClass { get; set; }
        }


    }
}
