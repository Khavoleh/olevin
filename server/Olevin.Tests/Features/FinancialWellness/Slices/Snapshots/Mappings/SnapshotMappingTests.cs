using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Mappings;

public sealed class SnapshotMappingTests
{
    private static readonly SnapshotScore Score = SnapshotScorer.Score(ControlExample.Snapshots())[
        ^1
    ];

    private static readonly SnapshotAmounts Amounts = ControlExample.SeptemberAmounts();

    private static Snapshot Map() =>
        SnapshotMapping.ToSnapshot(
            Guid.CreateVersion7(),
            new SnapshotMonth(2026, CalendarMonth.September),
            new DateOnly(2026, 9, 25),
            Amounts,
            4,
            Score
        );

    [Fact]
    public void Indicators_AreTheClampedOnes()
    {
        Snapshot snapshot = Map();

        Assert.Equal(Score.Inputs.Indicators.SavingsRate, snapshot.SavingsRate);
        Assert.Equal(Score.Inputs.Indicators.Reserve, snapshot.ReserveMonths);
        Assert.Equal(Score.Inputs.Indicators.DebtToIncome, snapshot.DebtToIncome);
        Assert.Equal(Score.Inputs.Indicators.ExpenseVariation, snapshot.ExpenseVariation);
        Assert.Equal(Score.Inputs.Indicators.SavingsRegularity, snapshot.SavingsRegularity);
    }

    [Fact]
    public void Indexes_AreTheOnesTheUserSees()
    {
        Snapshot snapshot = Map();

        Assert.Equal(Score.Index.CashFlow, snapshot.CashFlow);
        Assert.Equal(Score.Index.Buffer, snapshot.Buffer);
        Assert.Equal(Score.Index.FuzzyIndex, snapshot.FuzzyIndex);
        Assert.Equal(Score.Index.Linear, snapshot.LinearIndex);
        Assert.Equal(Score.Index.Final, snapshot.FinalIndex);
        Assert.Equal(Score.ModelVersion, snapshot.ModelVersion);
    }

    [Fact]
    public void Month_RatesAndAmounts_AreKept()
    {
        Snapshot snapshot = Map();

        Assert.Equal(CalendarMonth.September, snapshot.Month);
        Assert.Equal(2026, snapshot.Year);
        Assert.Equal(new DateOnly(2026, 9, 25), snapshot.FxDate);
        Assert.Equal(4, snapshot.SelfRating);
        Assert.Same(Amounts, snapshot.Amounts);
    }

    [Fact]
    public void EveryMapping_GetsANewId() => Assert.NotEqual(Map().Id, Map().Id);
}
