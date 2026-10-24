using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Scoring;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Helpers.Scoring;

public sealed class SnapshotScorerTests
{
    [Fact]
    public void Estimates_HaveNoScore_AndTheFirstActualSnapshotIsPreliminary()
    {
        SnapshotMonth current = new(2026, CalendarMonth.September);
        SnapshotFigures[] history =
        [
            new(new SnapshotMonth(2026, CalendarMonth.July), MonthFigures.Estimated(28_000, 2_000)),
            new(new SnapshotMonth(2026, CalendarMonth.August), MonthFigures.Estimated(32_000, 0)),
            new(
                current,
                MonthFigures.Actual(
                    50_000,
                    new ExpenseBreakdown(8_000, 10_000, 1_000, 2_000, 3_000, 2_000, 4_000),
                    3_000,
                    60_000,
                    0,
                    3
                )
            ),
        ];

        SnapshotScore score = Assert.Single(SnapshotScorer.Score(history));

        Assert.Equal(current, score.Month);
        Assert.True(score.Index.IsPreliminary);
    }

    [Fact]
    public void Index_StopsBeingPreliminaryAtTheThirdActualSnapshot()
    {
        IReadOnlyList<SnapshotScore> scores = SnapshotScorer.Score(ControlExample.Snapshots());

        Assert.Equal(
            [true, true, false, false, false, false],
            scores.Select(s => s.Index.IsPreliminary)
        );
    }

    [Fact]
    public void SameAmounts_GiveTheSameResults()
    {
        SnapshotScore first = SnapshotScorer.Score(ControlExample.Snapshots())[^1];
        SnapshotScore second = SnapshotScorer.Score(ControlExample.Snapshots())[^1];

        Assert.Equal(first.Inputs, second.Inputs);
        Assert.Equal(first.Index.FuzzyIndex, second.Index.FuzzyIndex);
        Assert.Equal(first.Index.Linear, second.Index.Linear);
        Assert.Equal(first.Index.Final, second.Index.Final);
    }

    [Fact]
    public void ChangingAnEarlierSnapshot_ChangesTheFollowingOnes()
    {
        SnapshotFigures[] history = ControlExample.Snapshots();
        SnapshotScore before = SnapshotScorer.Score(history)[^1];

        history[3] = history[3] with { Figures = history[3].Figures with { Saved = 0 } };
        SnapshotScore after = SnapshotScorer.Score(history)[^1];

        Assert.Equal(3.0 / 6, before.Inputs.Indicators.SavingsRegularity, 1e-9);
        Assert.Equal(2.0 / 6, after.Inputs.Indicators.SavingsRegularity, 1e-9);
        Assert.True(after.Index.FuzzyIndex < before.Index.FuzzyIndex);
    }
}
