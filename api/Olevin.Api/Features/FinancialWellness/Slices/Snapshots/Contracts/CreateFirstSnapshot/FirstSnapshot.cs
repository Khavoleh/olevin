using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.CreateFirstSnapshot;

/// <summary>
/// The first snapshot of a user, for the latest month whose snapshot day has come, with rough estimates of the 2 months before it.
/// </summary>
/// <param name="Income">The net income, possibly in several currencies.</param>
/// <param name="Expenses">The expenses by category, without debt payments.</param>
/// <param name="Saved">The amount saved during the month.</param>
/// <param name="Reserves">The liquid reserves at the end of the month, possibly in several currencies.</param>
/// <param name="DebtPayments">The monthly debt payments, possibly in several currencies.</param>
/// <param name="SelfRating">The self-rating of calm.</param>
/// <param name="PreviousMonth">The estimate of the month before the current one.</param>
/// <param name="TwoMonthsAgo">The estimate of the month before <paramref name="PreviousMonth"/>.</param>
public sealed record FirstSnapshot(
    IReadOnlyList<Money> Income,
    ExpensesRequest Expenses,
    Money Saved,
    IReadOnlyList<Money> Reserves,
    IReadOnlyList<Money> DebtPayments,
    int SelfRating,
    EstimateRequest PreviousMonth,
    EstimateRequest TwoMonthsAgo
);
