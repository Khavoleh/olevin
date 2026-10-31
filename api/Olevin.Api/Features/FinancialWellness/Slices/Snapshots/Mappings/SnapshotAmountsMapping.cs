using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;

/// <summary>
/// Maps the amounts of a snapshot from the request to the stored <see cref="SnapshotAmounts"/>.
/// </summary>
public static class SnapshotAmountsMapping
{
    /// <summary>
    /// Creates the stored amounts of an actual snapshot from the validated amounts.
    /// </summary>
    /// <param name="income">The net income.</param>
    /// <param name="expenses">The expenses by category.</param>
    /// <param name="saved">The amount saved during the month.</param>
    /// <param name="reserves">The liquid reserves at the end of the month.</param>
    /// <param name="debtPayments">The monthly debt payments.</param>
    /// <returns>The amounts as they are stored.</returns>
    public static SnapshotAmounts ToAmounts(
        IReadOnlyList<Money> income,
        ExpensesRequest expenses,
        Money saved,
        IReadOnlyList<Money> reserves,
        IReadOnlyList<Money> debtPayments
    ) =>
        new(
            [.. income],
            new ExpenseAmounts(
                expenses.Food,
                expenses.Housing,
                expenses.Health,
                expenses.Clothing,
                expenses.Restaurants,
                expenses.Transport,
                expenses.Other
            ),
            saved,
            [.. reserves],
            [.. debtPayments]
        );

    /// <summary>
    /// Lists the currencies the amounts were entered in.
    /// </summary>
    /// <param name="amounts">The amounts.</param>
    /// <returns>Every currency once.</returns>
    public static IEnumerable<CurrencyCode> Currencies(SnapshotAmounts amounts)
    {
        ExpenseAmounts expenses = amounts.Expenses;
        Money[] all =
        [
            .. amounts.Income,
            expenses.Food,
            expenses.Housing,
            expenses.Health,
            expenses.Clothing,
            expenses.Restaurants,
            expenses.Transport,
            expenses.Other,
            amounts.Saved,
            .. amounts.Reserves,
            .. amounts.DebtPayments,
        ];

        return all.Select(money => money.Currency).Distinct();
    }
}
