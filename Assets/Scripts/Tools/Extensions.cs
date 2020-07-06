using System;

public static class Extensions
{
    private static readonly DateTime _epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime FromUnixTime(long unixTime) => _epochStart.AddSeconds(unixTime);

    public static long ToUnixTime(this DateTime dateTime) => (long)(dateTime - _epochStart).TotalSeconds;

    public static string TimeSpanToString(TimeSpan timeSpan)
    {
        return timeSpan.Days > 0
            ? $"{timeSpan.Days} DAYS {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}"
            : $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
    }

    public static string TimeSpanToString(int timeSpanInSeconds)
    {
        return TimeSpanToString(TimeSpan.FromSeconds(timeSpanInSeconds));
    }
}