using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetIndex;

/// <summary>
/// The rules of the index shown to the user.
/// </summary>
public static class GetIndexRules
{
    /// <summary>
    /// Finds the day the next snapshot is due.
    /// </summary>
    /// <param name="latest">The month of the latest snapshot.</param>
    /// <param name="current">The current month in the user's time zone.</param>
    /// <param name="snapshotDay">The day of the month (1-31) from the user's settings.</param>
    /// <returns>The snapshot day of the current month, or of the next one when the current month is taken.</returns>
    public static DateOnly NextSnapshotDate(
        SnapshotMonth latest,
        SnapshotMonth current,
        short snapshotDay
    ) =>
        (latest.Year, latest.Month).CompareTo((current.Year, current.Month)) < 0
            ? current.DateOn(snapshotDay)
            : current.Next().DateOn(snapshotDay);
}
