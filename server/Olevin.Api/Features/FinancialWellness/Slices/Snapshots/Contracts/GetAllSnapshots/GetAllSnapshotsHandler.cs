using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots.DTOs;
using Wolverine.Attributes;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots;

/// <summary>
/// Gives other features a copy of every snapshot of a user.
/// </summary>
public static class GetAllSnapshotsHandler
{
    /// <summary>
    /// Handles <see cref="GetAllSnapshotsRequest"/>.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>Every snapshot of the user as stored, the oldest first; empty when the user has none.</returns>
    /// <remarks>It only reads, so it opts out of the automatic transactions.</remarks>
    [NonTransactional]
    public static async Task<IReadOnlyList<UserSnapshotDto>> Handle(
        GetAllSnapshotsRequest request,
        FinancialWellnessDbContext db,
        CancellationToken ct
    )
    {
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == request.UserId)
            .OrderBy(snapshot => snapshot.Year)
            .ThenBy(snapshot => snapshot.Month)
            .ToListAsync(ct);

        return
        [
            .. snapshots.Select(snapshot =>
            {
                SnapshotAmounts amounts = snapshot.Amounts;
                ExpenseAmounts expenses = amounts.Expenses;

                return new UserSnapshotDto(
                    snapshot.Id,
                    snapshot.Month.ToString(),
                    snapshot.Year,
                    snapshot.FxDate,
                    snapshot.SelfRating,
                    snapshot.SavingsRate,
                    snapshot.ReserveMonths,
                    snapshot.DebtToIncome,
                    snapshot.ExpenseVariation,
                    snapshot.SavingsRegularity,
                    snapshot.CashFlow,
                    snapshot.Buffer,
                    snapshot.FuzzyIndex,
                    snapshot.LinearIndex,
                    snapshot.FinalIndex,
                    [.. amounts.Income.Select(MoneyOf)],
                    new ExpensesDto(
                        MoneyOf(expenses.Food),
                        MoneyOf(expenses.Housing),
                        MoneyOf(expenses.Health),
                        MoneyOf(expenses.Clothing),
                        MoneyOf(expenses.Restaurants),
                        MoneyOf(expenses.Transport),
                        MoneyOf(expenses.Other)
                    ),
                    MoneyOf(amounts.Saved),
                    [.. amounts.Reserves.Select(MoneyOf)],
                    [.. amounts.DebtPayments.Select(MoneyOf)],
                    snapshot.ModelVersion,
                    snapshot.CreatedAt,
                    snapshot.UpdatedAt
                );
            }),
        ];
    }

    private static MoneyDto MoneyOf(Money money) => new(money.Amount, money.Currency);
}
