using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;

/// <summary>
/// The range of months a snapshot may be created for.
/// </summary>
public static class OpenMonths
{
    /// <summary>
    /// Finds the earliest month a snapshot may be created for.
    /// </summary>
    /// <param name="estimates">The months of the stored estimates.</param>
    /// <returns>The month after the latest estimate.</returns>
    public static SnapshotMonth Earliest(IEnumerable<SnapshotMonth> estimates) =>
        estimates.MaxBy(estimate => (estimate.Year, estimate.Month))!.Next();

    /// <summary>
    /// Finds the latest month whose snapshot day has come.
    /// </summary>
    /// <param name="today">The current date in the user's time zone.</param>
    /// <param name="snapshotDay">The day of the month (1-31) from the user's settings.</param>
    /// <returns>The current month from its snapshot day, the previous month before it.</returns>
    public static SnapshotMonth Latest(DateOnly today, short snapshotDay)
    {
        SnapshotMonth current = SnapshotMonth.Of(today);

        return today < current.DateOn(snapshotDay) ? current.Previous() : current;
    }
}
