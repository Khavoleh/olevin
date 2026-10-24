using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots;

/// <summary>
/// Finds the stored rates a snapshot is converted at.
/// </summary>
public static class FxRateLookup
{
    /// <summary>
    /// Finds the rates on the date, or on the closest date that has a rate for every needed currency.
    /// </summary>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="date">The date the snapshot should be converted at.</param>
    /// <param name="currencies">The currencies of the amounts and the base currency.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The rates with their actual date.</returns>
    public static async Task<SnapshotRates> FindAsync(
        FinancialWellnessDbContext db,
        DateOnly date,
        IEnumerable<CurrencyCode> currencies,
        CancellationToken ct
    )
    {
        List<CurrencyCode> needed = [.. currencies.Distinct()];

        IQueryable<DateOnly> complete = db
            .FxRates.Where(rate => needed.Contains(rate.Currency))
            .GroupBy(rate => rate.RateDate)
            .Where(group => group.Count() == needed.Count)
            .Select(group => group.Key);

        List<DateOnly> before = await complete
            .Where(rateDate => rateDate <= date)
            .OrderByDescending(rateDate => rateDate)
            .Take(1)
            .ToListAsync(ct);
        List<DateOnly> after = await complete
            .Where(rateDate => rateDate > date)
            .OrderBy(rateDate => rateDate)
            .Take(1)
            .ToListAsync(ct);

        DateOnly fxDate = Closest(date, [.. before, .. after]);

        Dictionary<CurrencyCode, decimal> unitsPerUsd = await db
            .FxRates.Where(rate => rate.RateDate == fxDate && needed.Contains(rate.Currency))
            .ToDictionaryAsync(rate => rate.Currency, rate => rate.UnitsPerBase, ct);

        return new SnapshotRates(fxDate, new ExchangeRates(unitsPerUsd));
    }

    /// <summary>
    /// Picks the candidate closest to a date.
    /// </summary>
    /// <param name="date">The wanted date.</param>
    /// <param name="candidates">The dates to choose from, earlier first.</param>
    /// <returns>The closest date, the first of them on a tie.</returns>
    public static DateOnly Closest(DateOnly date, IEnumerable<DateOnly> candidates) =>
        candidates.MinBy(candidate => Math.Abs(candidate.DayNumber - date.DayNumber));
}
