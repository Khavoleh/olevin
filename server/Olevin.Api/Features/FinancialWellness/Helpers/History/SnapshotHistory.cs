using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.FxRates;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Helpers.History;

/// <summary>
/// Builds the history of a user from the stored snapshots and estimates, in the base currency and in month order.
/// </summary>
public static class SnapshotHistory
{
    /// <summary>
    /// Builds the history at the stored rates of each month.
    /// </summary>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="estimates">The estimates of the user.</param>
    /// <param name="snapshots">The snapshots of the user, in any order.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The months in order.</returns>
    public static async Task<SnapshotFigures[]> LoadAsync(
        FinancialWellnessDbContext db,
        IReadOnlyList<EstimatedSnapshot> estimates,
        IReadOnlyList<Snapshot> snapshots,
        CurrencyCode baseCurrency,
        CancellationToken ct
    )
    {
        Dictionary<DateOnly, ExchangeRates> rates = await LoadRatesAsync(
            db,
            [
                .. estimates.Select(estimate => estimate.FxDate),
                .. snapshots.Select(snapshot => snapshot.FxDate),
            ],
            ct
        );

        return Build(estimates, snapshots, rates, baseCurrency);
    }

    /// <summary>
    /// Builds the history.
    /// </summary>
    /// <param name="estimates">The estimates of the user.</param>
    /// <param name="snapshots">The snapshots of the user, in any order.</param>
    /// <param name="rates">The rates by <c>FxDate</c> of every estimate and snapshot.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <returns>The months in order.</returns>
    public static SnapshotFigures[] Build(
        IEnumerable<EstimatedSnapshot> estimates,
        IEnumerable<Snapshot> snapshots,
        IReadOnlyDictionary<DateOnly, ExchangeRates> rates,
        CurrencyCode baseCurrency
    ) =>
        [
            .. estimates
                .Select(estimate =>
                    SnapshotHistoryMapping.ToFigures(estimate, rates[estimate.FxDate], baseCurrency)
                )
                .Concat(
                    snapshots.Select(snapshot =>
                        SnapshotHistoryMapping.ToFigures(
                            snapshot,
                            rates[snapshot.FxDate],
                            baseCurrency
                        )
                    )
                )
                .OrderBy(figures => (figures.Month.Year, figures.Month.Month)),
        ];

    private static async Task<Dictionary<DateOnly, ExchangeRates>> LoadRatesAsync(
        FinancialWellnessDbContext db,
        IEnumerable<DateOnly> dates,
        CancellationToken ct
    )
    {
        List<DateOnly> wanted = [.. dates.Distinct()];

        List<FxRate> rates = await db
            .FxRates.Where(rate => wanted.Contains(rate.RateDate))
            .ToListAsync(ct);

        return rates
            .GroupBy(rate => rate.RateDate)
            .ToDictionary(
                group => group.Key,
                group => new ExchangeRates(
                    group.ToDictionary(rate => rate.Currency, rate => rate.UnitsPerBase)
                )
            );
    }
}
