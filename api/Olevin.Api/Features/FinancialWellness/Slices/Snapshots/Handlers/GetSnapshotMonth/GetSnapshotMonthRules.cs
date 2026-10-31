using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshotMonth;

/// <summary>
/// The rules of one month of the history.
/// </summary>
public static class GetSnapshotMonthRules
{
    /// <summary>
    /// Counts the snapshots after the month.
    /// </summary>
    /// <param name="month">The month.</param>
    /// <param name="snapshots">The months of every snapshot.</param>
    /// <returns>The number of later snapshots.</returns>
    public static int CountLater(SnapshotMonth month, IEnumerable<SnapshotMonth> snapshots) =>
        snapshots.Count(other =>
            (other.Year, other.Month).CompareTo((month.Year, month.Month)) > 0
        );
}
