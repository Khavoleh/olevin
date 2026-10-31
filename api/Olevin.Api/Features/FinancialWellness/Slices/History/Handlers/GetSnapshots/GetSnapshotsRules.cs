using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.History.Handlers.GetSnapshots;

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
}
