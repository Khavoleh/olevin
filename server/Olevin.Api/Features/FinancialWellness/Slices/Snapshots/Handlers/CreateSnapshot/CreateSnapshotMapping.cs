using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;

/// <summary>
/// Maps the request and the stored months for scoring.
/// </summary>
public static class CreateSnapshotMapping
{
    /// <summary>
    /// Creates the stored amounts of the snapshot.
    /// </summary>
    /// <param name="request">The validated request.</param>
    /// <returns>The amounts as they are stored.</returns>
    public static SnapshotAmounts ToAmounts(CreateSnapshotRequest request) =>
        SnapshotAmountsMapping.ToAmounts(
            request.Income,
            request.Expenses,
            request.Saved,
            request.Reserves,
            request.DebtPayments
        );

    /// <summary>
    /// Gets the month of a stored snapshot.
    /// </summary>
    /// <param name="snapshot">The snapshot.</param>
    /// <returns>Its month.</returns>
    public static SnapshotMonth MonthOf(Snapshot snapshot) => new(snapshot.Year, snapshot.Month);

    /// <summary>
    /// Gets the month of a stored estimate.
    /// </summary>
    /// <param name="estimate">The estimate.</param>
    /// <returns>Its month.</returns>
    public static SnapshotMonth MonthOf(EstimatedSnapshot estimate) =>
        new(estimate.Year, estimate.Month);

    /// <summary>
    /// Converts a stored snapshot to figures in the base currency.
    /// </summary>
    /// <param name="snapshot">The snapshot.</param>
    /// <param name="rates">The rates on its <see cref="Snapshot.FxDate"/>.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <returns>The figures of the month.</returns>
    public static SnapshotFigures ToFigures(
        Snapshot snapshot,
        ExchangeRates rates,
        CurrencyCode baseCurrency
    ) =>
        new(
            MonthOf(snapshot),
            SnapshotAmountsMapping.ToFigures(
                snapshot.Amounts,
                rates,
                baseCurrency,
                snapshot.SelfRating
            )
        );

    /// <summary>
    /// Converts a stored estimate to figures in the base currency.
    /// </summary>
    /// <param name="estimate">The estimate.</param>
    /// <param name="rates">The rates on its <see cref="EstimatedSnapshot.FxDate"/>.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <returns>The figures of the month.</returns>
    public static SnapshotFigures ToFigures(
        EstimatedSnapshot estimate,
        ExchangeRates rates,
        CurrencyCode baseCurrency
    )
    {
        double ToBase(decimal amount, CurrencyCode currency) =>
            (double)rates.Convert(new Money(amount, currency), baseCurrency);

        return new SnapshotFigures(
            MonthOf(estimate),
            MonthFigures.Estimated(
                ToBase(estimate.ExpensesAmount, estimate.ExpensesCurrency),
                ToBase(estimate.SavedAmount, estimate.SavedCurrency)
            )
        );
    }
}
