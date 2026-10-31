using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Mappings;

public sealed class SnapshotAmountsMappingTests
{
    private static readonly SnapshotAmounts September = ControlExample.SeptemberAmounts();

    [Fact]
    public void ToAmounts_KeepsTheAmountsAsEntered()
    {
        ExpenseAmounts expenses = September.Expenses;

        SnapshotAmounts amounts = SnapshotAmountsMapping.ToAmounts(
            September.Income,
            new ExpensesRequest(
                expenses.Food,
                expenses.Housing,
                expenses.Health,
                expenses.Clothing,
                expenses.Restaurants,
                expenses.Transport,
                expenses.Other
            ),
            September.Saved,
            September.Reserves,
            September.DebtPayments
        );

        Assert.Equal(September.Income, amounts.Income);
        Assert.Equal(expenses, amounts.Expenses);
        Assert.Equal(September.Saved, amounts.Saved);
        Assert.Equal(September.Reserves, amounts.Reserves);
        Assert.Equal(September.DebtPayments, amounts.DebtPayments);
    }

    [Fact]
    public void Currencies_ListEveryCurrencyOnce() =>
        Assert.Equal(
            [CurrencyCode.UAH, CurrencyCode.USD],
            SnapshotAmountsMapping.Currencies(September)
        );
}
