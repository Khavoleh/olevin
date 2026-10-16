using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;

/// <summary>
/// Scores the whole history of a user again after one of their snapshots is added or changed.
/// </summary>
public static class SnapshotRescorer
{
    /// <summary>
    /// Scores every snapshot of the user at the stored rates of each month.
    /// </summary>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="estimates">All estimates of the user.</param>
    /// <param name="snapshots">All snapshots of the user with the changes applied, in any order.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The scores by month.</returns>
    public static async Task<Dictionary<SnapshotMonth, SnapshotScore>> ScoreAsync(
        FinancialWellnessDbContext db,
        IReadOnlyList<EstimatedSnapshot> estimates,
        IReadOnlyList<Snapshot> snapshots,
        CurrencyCode baseCurrency,
        CancellationToken ct
    )
    {
        Dictionary<DateOnly, ExchangeRates> rates = await FxRateLookup.LoadAsync(
            db,
            [
                .. estimates.Select(estimate => estimate.FxDate),
                .. snapshots.Select(snapshot => snapshot.FxDate),
            ],
            ct
        );

        return Score(estimates, snapshots, rates, baseCurrency);
    }

    /// <summary>
    /// Scores every snapshot of the user.
    /// </summary>
    /// <param name="estimates">All estimates of the user.</param>
    /// <param name="snapshots">All snapshots of the user, in any order.</param>
    /// <param name="rates">The rates by <c>FxDate</c> of every estimate and snapshot.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <returns>The scores by month.</returns>
    public static Dictionary<SnapshotMonth, SnapshotScore> Score(
        IEnumerable<EstimatedSnapshot> estimates,
        IEnumerable<Snapshot> snapshots,
        IReadOnlyDictionary<DateOnly, ExchangeRates> rates,
        CurrencyCode baseCurrency
    )
    {
        SnapshotFigures[] history =
        [
            .. estimates.Select(estimate =>
                SnapshotHistoryMapping.ToFigures(estimate, rates[estimate.FxDate], baseCurrency)
            ),
            .. snapshots.Select(snapshot =>
                SnapshotHistoryMapping.ToFigures(snapshot, rates[snapshot.FxDate], baseCurrency)
            ),
        ];

        return SnapshotScorer
            .Score([.. history.OrderBy(figures => (figures.Month.Year, figures.Month.Month))])
            .ToDictionary(score => score.Month);
    }

    /// <summary>
    /// Applies the new scores to the snapshots of the month and the later ones. Earlier months don't depend on it.
    /// </summary>
    /// <param name="snapshots">The stored snapshots.</param>
    /// <param name="from">The changed month.</param>
    /// <param name="scores">The new scores by month.</param>
    /// <param name="now">The time of the change.</param>
    public static void ApplyFrom(
        IEnumerable<Snapshot> snapshots,
        SnapshotMonth from,
        IReadOnlyDictionary<SnapshotMonth, SnapshotScore> scores,
        DateTime now
    )
    {
        foreach (
            Snapshot snapshot in snapshots.Where(stored =>
                (stored.Year, stored.Month).CompareTo((from.Year, from.Month)) >= 0
            )
        )
        {
            SnapshotMapping.ApplyScore(snapshot, scores[SnapshotHistoryMapping.MonthOf(snapshot)]);
            snapshot.UpdatedAt = now;
        }
    }
}
