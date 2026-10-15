using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Mappings;

public sealed class SnapshotAmountsMappingTests
{
    private const double Precision = 1e-9;

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
    public void ToFigures_ConvertsEveryAmountToTheBaseCurrency()
    {
        MonthFigures figures = SnapshotAmountsMapping.ToFigures(
            September,
            new ExchangeRates(ControlExample.UnitsPerUsd),
            CurrencyCode.UAH,
            4
        );

        Assert.Equal(ControlExample.Months()[^1], figures);
    }

    [Fact]
    public void ToFigures_SumsEveryEntryOfTheLists()
    {
        SnapshotAmounts amounts = September with
        {
            Income = [new Money(100, CurrencyCode.USD), ControlExample.Uah(415)],
            DebtPayments = [ControlExample.Uah(83), new Money(10, CurrencyCode.USD)],
        };

        MonthFigures figures = SnapshotAmountsMapping.ToFigures(
            amounts,
            new ExchangeRates(ControlExample.UnitsPerUsd),
            CurrencyCode.USD,
            4
        );

        Assert.Equal(110, figures.Income, Precision);
        Assert.Equal(12, figures.DebtPayments, Precision);
    }

    [Fact]
    public void Currencies_ListEveryCurrencyOnce() =>
        Assert.Equal(
            [CurrencyCode.UAH, CurrencyCode.USD],
            SnapshotAmountsMapping.Currencies(September)
        );
}
