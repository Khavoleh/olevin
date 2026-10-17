using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.UpdateSnapshot;

/// <summary>
/// The new values of a snapshot.
/// </summary>
/// <param name="Income">The net income, possibly in several currencies.</param>
/// <param name="Expenses">The expenses by category, without debt payments.</param>
/// <param name="Saved">The amount saved during the month.</param>
/// <param name="Reserves">The liquid reserves at the end of the month, possibly in several currencies.</param>
/// <param name="DebtPayments">The monthly debt payments, possibly in several currencies.</param>
/// <param name="SelfRating">The self-rating of calm.</param>
public sealed record UpdateSnapshotRequest(
    IReadOnlyList<Money> Income,
    ExpensesRequest Expenses,
    Money Saved,
    IReadOnlyList<Money> Reserves,
    IReadOnlyList<Money> DebtPayments,
    int SelfRating
);
