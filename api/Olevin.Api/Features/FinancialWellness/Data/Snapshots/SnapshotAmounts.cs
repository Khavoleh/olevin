namespace Olevin.Api.Features.FinancialWellness.Data.Snapshots;

/// <summary>
/// The amounts of a snapshot exactly as entered, each in its own currency; stored in <c>snapshots.amounts</c>.
/// Converted amounts are never stored, they are calculated from these and <see cref="Snapshot.FxDate"/>.
/// </summary>
/// <param name="Income">The net income of the month, up to 10, a currency may repeat.</param>
/// <param name="Expenses">The expenses of the month by category, without debt payments.</param>
/// <param name="Saved">The amount put aside in the month.</param>
/// <param name="Reserves">The liquid reserves at the end of the month, up to 10, a currency may repeat.</param>
/// <param name="DebtPayments">The monthly debt payments, up to 10, a currency may repeat.</param>
public sealed record SnapshotAmounts(
    IReadOnlyList<Money> Income,
    ExpenseAmounts Expenses,
    Money Saved,
    IReadOnlyList<Money> Reserves,
    IReadOnlyList<Money> DebtPayments
);
