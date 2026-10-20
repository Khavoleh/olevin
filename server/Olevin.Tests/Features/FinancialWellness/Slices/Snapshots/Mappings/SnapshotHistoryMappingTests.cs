using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Mappings;

public sealed class SnapshotHistoryMappingTests
{
    private const double Precision = 1e-9;

    private static readonly ExchangeRates Rates = new(ControlExample.UnitsPerUsd);

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
}
