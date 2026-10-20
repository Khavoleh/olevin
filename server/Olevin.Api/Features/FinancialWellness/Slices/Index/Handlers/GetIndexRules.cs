using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.Index.Handlers;

/// <summary>
/// The rules of the index shown to the user.
/// </summary>
public static class GetIndexRules
{
    /// <summary>
    /// Rounds an index to the one decimal it is shown with.
    /// </summary>
    /// <param name="index">The index from 0 to 100.</param>
    /// <returns>The rounded index.</returns>
    public static double Round(double index) => Math.Round(index, 1, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Finds the change of the index, so it matches the difference of the shown numbers.
    /// </summary>
    /// <param name="latest">The index of the latest snapshot.</param>
    /// <param name="previous">The index of the snapshot before it.</param>
    /// <returns>The difference of the rounded indexes, negative when the index fell.</returns>
    public static double Delta(double latest, double previous) =>
        Round(Round(latest) - Round(previous));

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
