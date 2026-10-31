using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Scoring;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.CreateFirstSnapshot;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Contracts.CreateFirstSnapshot;

public sealed class CreateFirstSnapshotMappingTests
{
    private const double Precision = 1e-9;

    private static readonly EstimateRequest Estimate = new(
        new Money(1_000, CurrencyCode.USD),
        ControlExample.Uah(6_000)
    );

    private static readonly SnapshotRates Rates = new(
        new DateOnly(2026, 8, 1),
        new ExchangeRates(ControlExample.UnitsPerUsd)
    );

    [Fact]
    public void ToEstimatedSnapshot_KeepsTheAmountsAsEntered()
    {
        Guid userId = Guid.CreateVersion7();

        EstimatedSnapshot estimated = CreateFirstSnapshotMapping.ToEstimatedSnapshot(
            userId,
            new SnapshotMonth(2026, CalendarMonth.July),
            Estimate,
            Rates
        );

        Assert.Equal(userId, estimated.UserId);
        Assert.Equal(CalendarMonth.July, estimated.Month);
        Assert.Equal(2026, estimated.Year);
        Assert.Equal(Rates.FxDate, estimated.FxDate);
        Assert.Equal(CurrencyCode.USD, estimated.ExpensesCurrency);
        Assert.Equal(1_000m, estimated.ExpensesAmount);
        Assert.Equal(CurrencyCode.UAH, estimated.SavedCurrency);
        Assert.Equal(6_000m, estimated.SavedAmount);
    }

    [Fact]
    public void ToFigures_ConvertsTheEstimateToTheBaseCurrency()
    {
        MonthFigures figures = CreateFirstSnapshotMapping.ToFigures(
            Estimate,
            Rates,
            CurrencyCode.UAH
        );

        Assert.True(figures.IsEstimated);
        Assert.Equal(1_000 * ControlExample.UsdRate, figures.Expenses, Precision);
        Assert.Equal(6_000, figures.Saved, Precision);
    }

    [Fact]
    public void Currencies_IncludeTheBaseCurrency() =>
        Assert.Equal(
            [CurrencyCode.USD, CurrencyCode.UAH, CurrencyCode.EUR],
            CreateFirstSnapshotMapping.Currencies(Estimate, CurrencyCode.EUR)
        );

    [Fact]
    public void TwoEstimatesAndTheFirstSnapshot_GiveAPreliminaryIndex()
    {
        SnapshotFigures[] history =
        [
            new(
                new SnapshotMonth(2026, CalendarMonth.July),
                MonthFigures.Estimated(ControlExample.Expenses[0], ControlExample.Saved[0])
            ),
            new(
                new SnapshotMonth(2026, CalendarMonth.August),
                MonthFigures.Estimated(ControlExample.Expenses[1], ControlExample.Saved[1])
            ),
            ControlExample.Snapshots()[^1],
        ];

        SnapshotScore score = Assert.Single(SnapshotScorer.Score(history));

        Assert.True(score.Index.IsPreliminary);
        Assert.Equal(3, score.Inputs.AvailableSnapshots);
    }
}
