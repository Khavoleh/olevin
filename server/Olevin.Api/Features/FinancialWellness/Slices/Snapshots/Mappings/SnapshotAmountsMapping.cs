using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;

/// <summary>
/// Maps the amounts of a snapshot between the request, the stored <see cref="SnapshotAmounts"/> and the figures of
/// the core.
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
    /// Converts the amounts of an actual snapshot to the figures of the core in the base currency.
    /// </summary>
    /// <param name="amounts">The amounts.</param>
    /// <param name="rates">The rates of the snapshot.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <param name="selfRating">The self-rating of calm from 1 to 5.</param>
    /// <returns>The figures of the month.</returns>
    public static MonthFigures ToFigures(
        SnapshotAmounts amounts,
        ExchangeRates rates,
        CurrencyCode baseCurrency,
        int selfRating
    )
    {
        double ToBase(Money money) => (double)rates.Convert(money, baseCurrency);

        ExpenseAmounts expenses = amounts.Expenses;

        return MonthFigures.Actual(
            amounts.Income.Sum(ToBase),
            new ExpenseBreakdown(
                ToBase(expenses.Food),
                ToBase(expenses.Housing),
                ToBase(expenses.Health),
                ToBase(expenses.Clothing),
                ToBase(expenses.Restaurants),
                ToBase(expenses.Transport),
                ToBase(expenses.Other)
            ),
            ToBase(amounts.Saved),
            amounts.Reserves.Sum(ToBase),
            amounts.DebtPayments.Sum(ToBase),
            selfRating
        );
    }

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
