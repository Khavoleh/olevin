using Olevin.Api.Features.FinancialWellness.Data;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;

/// <summary>
/// The month a snapshot belongs to.
/// </summary>
/// <param name="Year">The year.</param>
/// <param name="Month">The month.</param>
public sealed record SnapshotMonth(short Year, CalendarMonth Month)
{
    /// <summary>
    /// Gets the month that is current for the user right now.
    /// </summary>
    /// <param name="now">The current instant.</param>
    /// <param name="timezone">The IANA time zone name of the user.</param>
    /// <returns>The month in the user's time zone.</returns>
    public static SnapshotMonth Current(DateTimeOffset now, string timezone)
    {
        DateTimeOffset local = TimeZoneInfo.ConvertTime(
            now,
            TimeZoneInfo.FindSystemTimeZoneById(timezone)
        );

        return new SnapshotMonth((short)local.Year, (CalendarMonth)local.Month);
    }

    /// <summary>
    /// Gets the month before this one.
    /// </summary>
    /// <returns>The previous month, in the previous year for January.</returns>
    public SnapshotMonth Previous() =>
        Month == CalendarMonth.January
            ? new SnapshotMonth((short)(Year - 1), CalendarMonth.December)
            : new SnapshotMonth(Year, Month - 1);

    /// <summary>
    /// Gets the month after this one.
    /// </summary>
    /// <returns>The next month, in the next year for December.</returns>
    public SnapshotMonth Next() =>
        Month == CalendarMonth.December
            ? new SnapshotMonth((short)(Year + 1), CalendarMonth.January)
            : new SnapshotMonth(Year, Month + 1);

    /// <summary>
    /// Gets the snapshot day of the user in this month, which is the date of its exchange rates.
    /// </summary>
    /// <param name="snapshotDay">The day of the month (1-31) from the user's settings.</param>
    /// <returns>The date; the last day of the month when the month is shorter than <paramref name="snapshotDay"/>.</returns>
    public DateOnly DateOn(short snapshotDay) =>
        new(Year, (int)Month, Math.Min(snapshotDay, DateTime.DaysInMonth(Year, (int)Month)));
}
