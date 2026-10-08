using Olevin.Api.Features.Snapshots.Currency;
using Olevin.Api.Shared.Index;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// Maps requests to the figures of the core and the scores of the core to responses.
/// </summary>
public static class CalculateSnapshotsMapping
{
    /// <summary>
    /// Converts the amounts of a snapshot to the base currency.
    /// </summary>
    /// <param name="snapshot">The snapshot.</param>
    /// <param name="baseCurrency">The base currency.</param>
    /// <returns>The figures of the snapshot in the base currency.</returns>
    public static MonthFigures ToFigures(SnapshotRequest snapshot, CurrencyCode baseCurrency)
    {
        ExchangeRates rates = Rates(snapshot);

        double Convert(Money? money) =>
            money is null ? 0 : (double)rates.Convert(money, baseCurrency);

        if (snapshot.IsEstimated)
            return MonthFigures.Estimated(Convert(snapshot.ExpensesTotal), Convert(snapshot.Saved));

        ExpensesRequest expenses = snapshot.Expenses!;

        return MonthFigures.Actual(
            Convert(snapshot.Income?.Salary) + Convert(snapshot.Income?.Other),
            new ExpenseBreakdown(
                Convert(expenses.Food),
                Convert(expenses.Housing),
                Convert(expenses.Health),
                Convert(expenses.Clothing),
                Convert(expenses.Restaurants),
                Convert(expenses.Transport),
                Convert(expenses.Other)
            ),
            Convert(snapshot.Saved),
            snapshot.Reserves?.Sum(Convert) ?? 0,
            Convert(snapshot.DebtPayments),
            snapshot.SelfRating ?? IndexConstants.NeutralSelfRating
        );
    }

    /// <summary>
    /// Creates the response from the score of the core.
    /// </summary>
    /// <param name="score">The score.</param>
    /// <returns>The response.</returns>
    public static ScoreResponse ToResponse(SnapshotScore score)
    {
        IndexResult index = score.Index;

        return new ScoreResponse(
            score.Inputs,
            index.CashFlow,
            index.Buffer,
            index.FuzzyIndex,
            index.Linear,
            index.Final,
            index.Zone,
            Zones.Of(index.FuzzyIndex),
            Zones.Of(index.Linear),
            index.IsPreliminary,
            score.Change,
            score.ModelVersion
        );
    }

    /// <summary>
    /// Gets the rates of a snapshot.
    /// </summary>
    /// <param name="snapshot">The snapshot.</param>
    /// <returns>Its rates; only UAH if none are given.</returns>
    public static ExchangeRates Rates(SnapshotRequest snapshot) =>
        new(snapshot.FxRates ?? new Dictionary<CurrencyCode, decimal>());

    /// <summary>
    /// Lists every amount of a snapshot.
    /// </summary>
    /// <param name="snapshot">The snapshot.</param>
    /// <returns>The amounts, <see langword="null"/> where missing.</returns>
    public static IEnumerable<Money?> Amounts(SnapshotRequest snapshot)
    {
        return
        [
            snapshot.ExpensesTotal,
            snapshot.Saved,
            snapshot.Income?.Salary,
            snapshot.Income?.Other,
            snapshot.Expenses?.Food,
            snapshot.Expenses?.Housing,
            snapshot.Expenses?.Health,
            snapshot.Expenses?.Clothing,
            snapshot.Expenses?.Restaurants,
            snapshot.Expenses?.Transport,
            snapshot.Expenses?.Other,
            snapshot.DebtPayments,
            .. snapshot.Reserves ?? [],
        ];
    }
}
