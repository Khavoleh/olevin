using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.History.Handlers.DeleteSnapshot;

/// <summary>
/// The snapshot rules for the deletion.
/// </summary>
public static class DeleteSnapshotRules
{
    /// <summary>
    /// Checks whether the deleted month is the latest one, so no later snapshot depends on it.
    /// </summary>
    /// <param name="month">The month of the deleted snapshot.</param>
    /// <param name="remaining">The months of the snapshots that stay.</param>
    /// <returns><see langword="true"/> when no remaining snapshot is later.</returns>
    public static bool IsLatest(SnapshotMonth month, IEnumerable<SnapshotMonth> remaining) =>
        remaining.All(other => (other.Year, other.Month).CompareTo((month.Year, month.Month)) < 0);
}
