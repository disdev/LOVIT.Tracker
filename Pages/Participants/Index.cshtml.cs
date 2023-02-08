using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LOVIT.Tracker.Services;
using LOVIT.Tracker.Models;
using LOVIT.Tracker.Utilities;

namespace LOVIT.Tracker.Pages.Participants;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IParticipantService _participantService;
    private readonly ISegmentService _segmentService;
    private readonly ICheckinService _checkinService;
    private readonly IAuth0Service _auth0Service;
    private readonly IWatcherService _watcherService;
    private readonly IRaceService _raceService;
    private readonly IPredictionService _predictionService;

    public IndexModel(ILogger<IndexModel> logger, IParticipantService participantService, ISegmentService segmentService, ICheckinService checkinService, IAuth0Service auth0Service, IWatcherService watcherService, IRaceService raceService, IPredictionService predictionService)
    {
        _logger = logger;
        _participantService = participantService;
        _segmentService = segmentService;
        _checkinService = checkinService;
        _auth0Service = auth0Service;
        _watcherService = watcherService;
        _raceService = raceService;
        _predictionService = predictionService;
    }

    public Race Race { get; set; } = new();
    public List<Segment> Segments { get; set; } = new();
    public Participant Participant { get; set; } = new();
    public List<ParticipantDetailsViewModel> ParticipantData { get; set; } = new();
    public bool PhoneNumberSet { get; set; } = false;
    public bool ShowNotifyOption { get; set; } = true;
    public bool IsAuthenticated { get; set; } = false;

    public async Task OnGet(string id)
    {
        Participant = await _participantService.GetParticipantAsync(id, true);
        Race = await _raceService.GetRaceAsync(Participant.RaceId);
        //await LoadData(id);
        await SetNotificationOptions();
        ViewData["Title"] = Participant.FullName;
    }

    private async Task SetNotificationOptions()
    {
        IsAuthenticated = User.Identity.IsAuthenticated;
        var userId = User.Claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
        if (User.Identity.IsAuthenticated && !String.IsNullOrEmpty(await _auth0Service.GetUserPhoneNumber(userId)))
        {
            PhoneNumberSet = true;

            if (await _watcherService.WatcherExists(Participant.Id, userId) == true)
            {
                ShowNotifyOption = false;
            }
        }
    }

    public async Task LoadData(string participantId)
    {
        Segments = await _segmentService.GetSegmentsAsync(Participant.RaceId);
        
        if (Participant.Status != Status.Registered || Participant.Status != Status.DNS)
        {
            Participant = await _participantService.GetParticipantWithCheckinsAsync(Participant.Id);
        }

        //var predictions = new List<SegmentPredictionModelInput>();
        if (Participant.Status == Status.Registered || (Participant.Status == Status.Started && Participant.Checkins.Count == 0))
        {
            //predictions = await _predictionService.GetEstimatesAsync(Participant, Segments, Race.Code, 0);
        }
        else if (Participant.Status == Status.Started)
        {
            // get segments without checkins
            var checkinSegmentIds = Participant.Checkins.Where(x => x.Confirmed == true).Select(x => x.SegmentId).ToList();
            var missingSegments = Segments.Where(x => !checkinSegmentIds.Contains(x.Id)).OrderBy(x => x.Order).ToList();
            var lastCheckin = Participant.Checkins.OrderBy(x => x.When).Last();
            //predictions = await _predictionService.GetEstimatesAsync(Participant, missingSegments, Race.Code, (lastCheckin.When - Race.Start).TotalSeconds);
        }

        Int64 lastCheckinOverallPace = 0;
        DateTime lastCheckinDateTime = new DateTime();

        foreach (var segment in Segments)
        {
            var detail = new ParticipantDetailsViewModel();

            detail.Order = segment.Order;
            detail.SegmentName = segment.Name;
            detail.SegmentId = segment.Id;
            detail.Distance = segment.Distance;
            detail.TotalDistance = segment.TotalDistance;

            var checkin = Participant.Checkins.Where(x => x.SegmentId == segment.Id && x.Confirmed == true).FirstOrDefault();
            if (checkin != null)
            {
                detail.CheckinDateTime = checkin.When;

                if (segment.Order == 1)
                {
                    detail.SegmentTime = TimeHelpers.FormatTime(Race.Start, checkin.When); //(checkin.When - participantWithCheckins.Race.Start).ToString(@"h\:mm\:ss");
                    detail.SegmentPace = TimeHelpers.CalculatePace(Race.Start, checkin.When, segment.Distance);
                }
                else
                {
                    detail.SegmentTime = TimeHelpers.FormatTime(lastCheckinDateTime, checkin.When); //(checkin.When - lastCheckinDateTime).ToString(@"h\:mm\:ss");
                    detail.SegmentPace = TimeHelpers.CalculatePace(lastCheckinDateTime, checkin.When, segment.Distance);
                }

                lastCheckinOverallPace = TimeHelpers.CalculatePaceInSeconds(Race.Start, checkin.When, segment.TotalDistance);
                detail.OverallTime = TimeHelpers.FormatTime(Race.Start, checkin.When); //(checkin.When - participantWithCheckins.Race.Start).ToString(@"h\:mm\:ss");
                detail.OverallPace = TimeHelpers.FormatPace(lastCheckinOverallPace);

                lastCheckinDateTime = checkin.When;
            }
            else if (Participant.Status == Status.Started || Participant.Status == Status.Registered)
            {
                /*
                detail.Estimated = true;

                // get the estimate
                var prediction = predictions.Single(x => x.SegmentOrder == segment.Order);
                DateTime predictionWhen = Race.Start.AddSeconds(prediction.LastTotalElapsed + prediction.SegmentElapsed);

                detail.CheckinDateTime = predictionWhen;

                if (segment.Order == 1)
                {
                    detail.SegmentTime = TimeHelpers.FormatTime(Race.Start, predictionWhen); //(checkin.When - participantWithCheckins.Race.Start).ToString(@"h\:mm\:ss");
                    detail.SegmentPace = TimeHelpers.CalculatePace(Race.Start, predictionWhen, segment.Distance);
                }
                else
                {
                    detail.SegmentTime = TimeHelpers.FormatTime(lastCheckinDateTime, predictionWhen); //(checkin.When - lastCheckinDateTime).ToString(@"h\:mm\:ss");
                    detail.SegmentPace = TimeHelpers.CalculatePace(lastCheckinDateTime, predictionWhen, segment.Distance);
                }

                detail.OverallTime = TimeHelpers.FormatTime(Race.Start, detail.CheckinDateTime); //(detail.CheckinDateTime - participantWithCheckins.Race.Start).ToString(@"h\:mm");
                detail.OverallPace = TimeHelpers.FormatPace(TimeHelpers.CalculatePaceInSeconds(Race.Start, detail.CheckinDateTime, segment.TotalDistance));

                lastCheckinDateTime = detail.CheckinDateTime;
                */
            }/*
            else if (Participant.Status == Status.Registered)
            {
                detail.Estimated = true;

                var prediction = predictions.Single(x => x.SegmentOrder == segment.Order);
                DateTime predictionWhen = Race.Start.AddSeconds(prediction.LastTotalElapsed + prediction.SegmentElapsed);

                detail.CheckinDateTime = predictionWhen;
                detail.SegmentTime = TimeHelpers.FormatSeconds((int)prediction.SegmentElapsed);
                detail.SegmentPace = TimeHelpers.CalculatePace((int)prediction.SegmentElapsed, prediction.SegmentDistance);

                detail.OverallTime = TimeHelpers.FormatTime(Race.Start, predictionWhen);
                detail.OverallPace = TimeHelpers.FormatPace(TimeHelpers.CalculatePaceInSeconds(Race.Start, predictionWhen, segment.TotalDistance));
            }*/

            ParticipantData.Add(detail);
        }
    }

    public class ParticipantDetailsViewModel
    {
        public int Order { get; set; }
        public string SegmentName { get; set; } = "";
        public Guid SegmentId { get; set; }
        public double Distance { get; set; }
        public double TotalDistance { get; set; }
        public DateTime CheckinDateTime { get; set; }
        public string SegmentTime { get; set; } = "";
        public string SegmentPace { get; set; } = "";
        public string OverallTime { get; set; } = "";
        public string OverallPace { get; set; } = "";
        public bool Estimated { get; set; }
    }
}

