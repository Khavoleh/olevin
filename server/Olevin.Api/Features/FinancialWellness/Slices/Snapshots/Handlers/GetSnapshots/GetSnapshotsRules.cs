using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshots;

/// <summary>
/// The rules of the snapshot history.
/// </summary>
public static class GetSnapshotsRules
{
    /// <summary>
    /// Finds the change of the index since the previous snapshot.
    /// </summary>
    /// <param name="newestFirst">The final indexes of every snapshot, the latest first.</param>
    /// <param name="position">The position of the snapshot in <paramref name="newestFirst"/>.</param>
    /// <returns>The change, or <see langword="null"/> for the first snapshot.</returns>
    public static double? ChangeAt(IReadOnlyList<double> newestFirst, int position) =>
        position + 1 < newestFirst.Count
            ? IndexPoints.Delta(newestFirst[position], newestFirst[position + 1])
            : null;

    /// <summary>
    /// Rounds an amount in the base currency.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <returns>The amount rounded to two decimals, halves away from zero.</returns>
    public static double RoundMoney(double amount) =>
        Math.Round(amount, 2, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Finds when a new snapshot can be created.
    /// </summary>
    /// <param name="earliest">The earliest month a snapshot may be created for.</param>
    /// <param name="latest">The latest month whose snapshot day has come.</param>
    /// <param name="taken">The months of the stored snapshots.</param>
    /// <param name="snapshotDay">The day of the month (1-31) from the user's settings.</param>
    /// <returns>
    /// The snapshot day of the first free month, or <see langword="null"/> when that month is already open.
    /// </returns>
    public static DateOnly? NewSnapshotOpensOn(
        SnapshotMonth earliest,
        SnapshotMonth latest,
        IReadOnlyCollection<SnapshotMonth> taken,
        short snapshotDay
    )
    {
        SnapshotMonth month = earliest;

        while (taken.Contains(month))
            month = month.Next();

        return (month.Year, month.Month).CompareTo((latest.Year, latest.Month)) <= 0
            ? null
            : month.DateOn(snapshotDay);
    }
}
