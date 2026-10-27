using Olevin.Api.Features.Settings.Data.PushSubscriptions;

namespace Olevin.Api.Features.Settings.Slices.SnapshotDayPush;

/// <summary>
/// When the push on the snapshot day is due and how long it stays useful.
/// </summary>
public static class SnapshotDayPushSchedule
{
    /// <summary>
    /// The local time of the snapshot day the push is sent from.
    /// </summary>
    public static readonly TimeOnly SendFrom = new(12, 0);

    /// <summary>
    /// Finds the snapshot day the push is due on now.
    /// </summary>
    /// <param name="now">The current instant.</param>
    /// <param name="timezone">The IANA time zone name of the user.</param>
    /// <param name="snapshotDay">The day of the month (1-31) from the user's settings.</param>
    /// <returns>
    /// Today in the user's time zone when it is the snapshot day and its noon has come, otherwise
    /// <see langword="null"/>; the snapshot day is the last day of a month shorter than <paramref name="snapshotDay"/>.
    /// </returns>
    public static DateOnly? DueDate(DateTimeOffset now, string timezone, short snapshotDay)
    {
        DateTime local = TimeZoneInfo
            .ConvertTime(now, TimeZoneInfo.FindSystemTimeZoneById(timezone))
            .DateTime;
        DateOnly today = DateOnly.FromDateTime(local);
        int day = Math.Min(snapshotDay, DateTime.DaysInMonth(today.Year, today.Month));

        return today.Day == day && TimeOnly.FromDateTime(local) >= SendFrom ? today : null;
    }

    /// <summary>
    /// Checks whether a subscription already got the push of a snapshot day.
    /// </summary>
    /// <param name="snapshotDay">The snapshot day the push is due on.</param>
    /// <param name="year">The year of the last push of the subscription.</param>
    /// <param name="month">The month of the last push of the subscription.</param>
    /// <returns>Whether the last push was sent in the month of <paramref name="snapshotDay"/>.</returns>
    public static bool IsSent(DateOnly snapshotDay, short? year, CalendarMonth? month) =>
        year == snapshotDay.Year && month == (CalendarMonth)snapshotDay.Month;

    /// <summary>
    /// Finds how long the push of a snapshot day stays useful.
    /// </summary>
    /// <param name="snapshotDay">The snapshot day of the push.</param>
    /// <param name="now">The current instant.</param>
    /// <param name="timezone">The IANA time zone name of the user.</param>
    /// <returns>The time left until the end of the snapshot day in the user's time zone; negative after it.</returns>
    public static TimeSpan TimeLeft(DateOnly snapshotDay, DateTimeOffset now, string timezone)
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        DateTime midnight = snapshotDay.AddDays(1).ToDateTime(TimeOnly.MinValue);

        return new DateTimeOffset(midnight, zone.GetUtcOffset(midnight)) - now;
    }
}
