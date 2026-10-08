using Olevin.Api.Features.Snapshots.Currency;

namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// One monthly snapshot. An estimate has only <see cref="ExpensesTotal"/> and <see cref="Saved"/>; an actual
/// snapshot has everything except <see cref="ExpensesTotal"/>.
/// </summary>
/// <param name="Month">Any day of the month of the snapshot.</param>
/// <param name="IsEstimated">Whether this is an approximate month from onboarding.</param>
/// <param name="FxRates">
/// The number of hryvnias per unit of every other currency used in the snapshot or chosen as the base, on the
/// date of the snapshot.
/// </param>
/// <param name="ExpensesTotal">The approximate total expenses of an estimate.</param>
/// <param name="Saved">The amount saved during the month.</param>
/// <param name="Income">The net income.</param>
/// <param name="Expenses">The expenses by category, without debt payments.</param>
/// <param name="Reserves">The liquid reserves at the end of the month, possibly in several currencies.</param>
/// <param name="DebtPayments">The monthly debt payments; none if omitted.</param>
/// <param name="SelfRating">The self-rating of calm from 1 to 5.</param>
public sealed record SnapshotRequest(
    DateOnly Month,
    bool IsEstimated,
    IReadOnlyDictionary<CurrencyCode, decimal>? FxRates,
    Money? ExpensesTotal,
    Money? Saved,
    IncomeRequest? Income,
    ExpensesRequest? Expenses,
    IReadOnlyList<Money>? Reserves,
    Money? DebtPayments,
    int? SelfRating
);
