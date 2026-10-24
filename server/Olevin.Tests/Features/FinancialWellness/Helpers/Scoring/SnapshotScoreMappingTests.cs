using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.Scoring;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Helpers.Scoring;

public sealed class SnapshotScoreMappingTests
{
    private static readonly SnapshotScore Score = SnapshotScorer.Score(ControlExample.Snapshots())[
        ^1
    ];

    private static readonly SnapshotAmounts Amounts = ControlExample.SeptemberAmounts();

    private static Snapshot Scored()
    {
        Snapshot snapshot = new()
        {
            Id = Guid.CreateVersion7(),
            Year = 2026,
            Month = CalendarMonth.September,
            FxDate = new DateOnly(2026, 9, 25),
            SelfRating = 4,
            Amounts = Amounts,
        };

        SnapshotScoreMapping.ApplyScore(snapshot, Score);

        return snapshot;
    }

    [Fact]
    public void Indicators_AreTheClampedOnes()
    {
        Snapshot snapshot = Scored();

        Assert.Equal(Score.Inputs.Indicators.SavingsRate, snapshot.SavingsRate);
        Assert.Equal(Score.Inputs.Indicators.Reserve, snapshot.ReserveMonths);
        Assert.Equal(Score.Inputs.Indicators.DebtToIncome, snapshot.DebtToIncome);
        Assert.Equal(Score.Inputs.Indicators.ExpenseVariation, snapshot.ExpenseVariation);
        Assert.Equal(Score.Inputs.Indicators.SavingsRegularity, snapshot.SavingsRegularity);
    }

    [Fact]
    public void Indexes_AreTheOnesTheUserSees()
    {
        Snapshot snapshot = Scored();

        Assert.Equal(Score.Index.CashFlow, snapshot.CashFlow);
        Assert.Equal(Score.Index.Buffer, snapshot.Buffer);
        Assert.Equal(Score.Index.FuzzyIndex, snapshot.FuzzyIndex);
        Assert.Equal(Score.Index.Linear, snapshot.LinearIndex);
        Assert.Equal(Score.Index.Final, snapshot.FinalIndex);
        Assert.Equal(Score.ModelVersion, snapshot.ModelVersion);
    }

    [Fact]
    public void ApplyScore_ReplacesTheCalculatedPartOnly()
    {
        SnapshotScore august = SnapshotScorer.Score(ControlExample.Snapshots())[^2];
        Snapshot snapshot = Scored();
        Guid id = snapshot.Id;

        SnapshotScoreMapping.ApplyScore(snapshot, august);

        Assert.Equal(august.Inputs.Indicators.ExpenseVariation, snapshot.ExpenseVariation);
        Assert.Equal(august.Inputs.Indicators.SavingsRegularity, snapshot.SavingsRegularity);
        Assert.Equal(august.Index.Buffer, snapshot.Buffer);
        Assert.Equal(august.Index.Final, snapshot.FinalIndex);
        Assert.Equal(august.ModelVersion, snapshot.ModelVersion);
        Assert.Equal(id, snapshot.Id);
        Assert.Equal(CalendarMonth.September, snapshot.Month);
        Assert.Equal(4, snapshot.SelfRating);
        Assert.Same(Amounts, snapshot.Amounts);
    }
}
