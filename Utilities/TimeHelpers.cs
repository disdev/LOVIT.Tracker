namespace LOVIT.Tracker.Utilities;

public static class TimeHelpers
{
    public static string CalculatePace(DateTime start, DateTime end, double distance)
    {
        var secondsPerMile = CalculatePaceInSeconds(start, end, distance);
        return FormatPace(secondsPerMile);
    }

    public static string CalculatePace(Int64 elapsedInSeconds, double distance)
    {
        var secondsPerMile = Convert.ToInt64(elapsedInSeconds / distance);
        return FormatPace(secondsPerMile);
    }

    public static Int64 CalculatePaceInSeconds(Int64 elapsedInSeconds, double distance)
    {
        return Convert.ToInt64(elapsedInSeconds / distance);
    }

    public static Int64 CalculatePaceInSeconds(DateTime start, DateTime end, double distance)
    {
        var elapsed = (end - start).TotalSeconds;
        var secondsPerMile = Convert.ToInt64(elapsed / distance);

        return secondsPerMile;
    }

    public static string FormatPace(Int64 paceInSeconds)
    {
        var minutes = (paceInSeconds / 60).ToString();
        var seconds = (paceInSeconds % 60).ToString("00");
        return $"{minutes}:{seconds}";
    }

    public static string FormatTime(DateTime start, DateTime end)
    {
        return FormatSpan(end - start);
    }

    public static string FormatSeconds(uint seconds)
    {
        return FormatSpan(TimeSpan.FromSeconds(seconds));
    }

    public static string FormatSpan(TimeSpan? span)
    {
        if (span.HasValue && span.Value.TotalSeconds > 0)
        {
            if (span.Value.TotalSeconds >= (60 * 60))
            {
                return $"{(span.Value.Days * 24) + span.Value.Hours}:{span.Value.ToString(@"mm\:ss")}";
            }

            return span.Value.ToString(@"mm\:ss");
        }

        return "";
    }

    public static DateTime CalculateEstimatedTime(DateTime start, double distance, Int64 paceInSeconds)
    {
        var estimatedElapsed = Convert.ToInt64(paceInSeconds * distance);
        return start.AddSeconds(estimatedElapsed);
    }
}