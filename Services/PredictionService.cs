using LOVIT.Tracker.Models;
using static LOVIT.Tracker.Pages.Participants.IndexModel;
using Newtonsoft.Json;

namespace LOVIT.Tracker.Services;

public interface IPredictionService
{
    Task<SegmentPredictionModelInput> GetEstimateAsync(Participant participant, Segment segment, string raceCode, double lastTotalElapsed);
    Task<List<SegmentPredictionModelInput>> GetEstimatesAsync(Participant participant, List<Segment> segments, string raceCode, double lastTotalElapsed);
}

public class PredictionService : IPredictionService
{
    public const string BASE_URL = "https://lovit-tracker-predictions.azurewebsites.net/";

    public async Task<SegmentPredictionModelInput> GetEstimateAsync(Participant participant, Segment segment, string raceCode, double lastTotalElapsed)
    {
        var input = new SegmentPredictionModelInput()
        {
            FullName = participant.FullName,
            Age = participant.Age,
            Rank = participant.Rank,
            Gender = (int)participant.Gender,
            RaceCode = raceCode,
            SegmentOrder = segment.Order,
            SegmentDistance = (float)segment.Distance,
            TotalDistance = (float)segment.TotalDistance,
            LastTotalElapsed = (float)lastTotalElapsed
        };
        
        var resultString = await SendRequest<SegmentPredictionModelInput>(input, "estimate/single");
        return JsonConvert.DeserializeObject<SegmentPredictionModelInput>(resultString);
    }

    public async Task<List<SegmentPredictionModelInput>> GetEstimatesAsync(Participant participant, List<Segment> segments, string raceCode, double lastTotalElapsed)
    {
        var inputs = new List<SegmentPredictionModelInput>();

        foreach (var segment in segments.OrderBy(x => x.Order))
        {
            inputs.Add(new SegmentPredictionModelInput() {
                FullName = participant.FullName,
                Age = participant.Age,
                Rank = participant.Rank,
                Gender = (int)participant.Gender,
                RaceCode = raceCode,
                SegmentOrder = segment.Order,
                SegmentDistance = (float)segment.Distance,
                TotalDistance = (float)segment.TotalDistance,
                LastTotalElapsed = 0F,
                SegmentElapsed = 0F
            });
        }

        inputs.First().LastTotalElapsed = (float)lastTotalElapsed;

        var resultString = await SendRequest<List<SegmentPredictionModelInput>>(inputs, "estimate/multiple");
        return JsonConvert.DeserializeObject<List<SegmentPredictionModelInput>>(resultString);
    }

    private async Task<string> SendRequest<T>(T bodyObject, string actionUrl)
    {
        /*
        var client = new HttpClient();
        var body = JsonConvert.SerializeObject(bodyObject);
        
        var response = await client.PostAsync($"{BASE_URL}/{actionUrl}", new StringContent(body, System.Text.Encoding.Default, "application/json"));
        return await response.Content.ReadAsStringAsync();
        */
        return "";
    }
}

