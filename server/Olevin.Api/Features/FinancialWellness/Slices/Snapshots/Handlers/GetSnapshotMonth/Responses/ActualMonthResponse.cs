using Olevin.Api.Features.FinancialWellness.Data.Snapshots;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshotMonth.Responses;

/// <summary>
/// The amounts of a snapshot as entered.
/// </summary>
/// <param name="Id">The snapshot identifier.</param>
/// <param name="LaterSnapshots">The number of snapshots after the month, scored again when it changes.</param>
/// <param name="Income">The net income, possibly in several currencies.</param>
/// <param name="Expenses">The expenses by category, without debt payments.</param>
/// <param name="Saved">The amount saved during the month.</param>
/// <param name="Reserves">The liquid reserves at the end of the month, possibly in several currencies.</param>
/// <param name="DebtPayments">The monthly debt payments, possibly in several currencies.</param>
/// <param name="SelfRating">The self-rating of calm from 1 to 5.</param>
public sealed record ActualMonthResponse(
    Guid Id,
    int LaterSnapshots,
    IReadOnlyList<Money> Income,
    ExpensesResponse Expenses,
    Money Saved,
    IReadOnlyList<Money> Reserves,
    IReadOnlyList<Money> DebtPayments,
    int SelfRating
) : GetSnapshotMonthResponse(Id, LaterSnapshots);
