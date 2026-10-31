using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Helpers.History;

public sealed class SnapshotHistoryMappingTests
{
    private const double Precision = 1e-9;

    private static readonly ExchangeRates Rates = new(ControlExample.UnitsPerUsd);

    private static readonly SnapshotAmounts September = ControlExample.SeptemberAmounts();

    [Fact]
    public void ToFigures_OfAnEstimate_ConvertsItToTheBaseCurrency()
    {
        EstimatedSnapshot estimate = new()
        {
            Year = 2026,
            Month = CalendarMonth.August,
            ExpensesCurrency = CurrencyCode.USD,
            ExpensesAmount = 1_000,
            SavedCurrency = CurrencyCode.UAH,
            SavedAmount = 6_000,
        };

        SnapshotFigures figures = SnapshotHistoryMapping.ToFigures(
            estimate,
            Rates,
            CurrencyCode.UAH
        );

        Assert.Equal(new SnapshotMonth(2026, CalendarMonth.August), figures.Month);
        Assert.True(figures.Figures.IsEstimated);
        Assert.Equal(1_000 * ControlExample.UsdRate, figures.Figures.Expenses, Precision);
        Assert.Equal(6_000, figures.Figures.Saved, Precision);
    }

    [Fact]
    public void ToFigures_OfAStoredSnapshot_UsesItsAmountsAndSelfRating()
    {
        Snapshot snapshot = new()
        {
            Year = 2026,
            Month = CalendarMonth.September,
            SelfRating = 4,
            Amounts = ControlExample.SeptemberAmounts(),
        };

        SnapshotFigures figures = SnapshotHistoryMapping.ToFigures(
            snapshot,
            Rates,
            CurrencyCode.UAH
        );

        Assert.Equal(new SnapshotMonth(2026, CalendarMonth.September), figures.Month);
        Assert.False(figures.Figures.IsEstimated);
        Assert.Equal(60_000, figures.Figures.Income, Precision);
        Assert.Equal(ControlExample.September.Total, figures.Figures.Expenses, Precision);
        Assert.Equal(
            (2_000 * ControlExample.UsdRate) + 106_000,
            figures.Figures.Reserves,
            Precision
        );
        Assert.Equal(4, figures.Figures.SelfRating);
    }

    [Fact]
    public void ToFigures_ConvertsEveryAmountToTheBaseCurrency()
    {
        MonthFigures figures = SnapshotHistoryMapping.ToFigures(
            September,
            Rates,
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

        MonthFigures figures = SnapshotHistoryMapping.ToFigures(
            amounts,
            Rates,
            CurrencyCode.USD,
            4
        );

        Assert.Equal(110, figures.Income, Precision);
        Assert.Equal(12, figures.DebtPayments, Precision);
    }
}
