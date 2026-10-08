using Olevin.Api.Features.Snapshots;
using Olevin.Api.Features.Snapshots.Changes;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.Snapshots;

public sealed class SnapshotScorerTests
{
    [Fact]
    public void Estimates_HaveNoScore_AndTheFirstActualSnapshotIsPreliminary()
    {
        MonthFigures[] months =
        [
            MonthFigures.Estimated(28_000, 2_000),
            MonthFigures.Estimated(32_000, 0),
            MonthFigures.Actual(
                50_000,
                new ExpenseBreakdown(8_000, 10_000, 1_000, 2_000, 3_000, 2_000, 4_000),
                3_000,
                60_000,
                0,
                3
            ),
        ];

        IReadOnlyList<SnapshotScore?> scores = SnapshotScorer.Score(months);

        Assert.Null(scores[0]);
        Assert.Null(scores[1]);
        Assert.NotNull(scores[2]);
        Assert.True(scores[2]!.Index.IsPreliminary);
        Assert.Null(scores[2]!.Change);
    }

    [Fact]
    public void Index_StopsBeingPreliminaryAtTheThirdActualSnapshot()
    {
        IReadOnlyList<SnapshotScore?> scores = SnapshotScorer.Score(ControlExample.Months());

        Assert.Equal(
            [true, true, false, false, false, false],
            scores.Select(s => s!.Index.IsPreliminary)
        );
    }

    [Fact]
    public void EveryActualSnapshotAfterTheFirst_ExplainsItsChange()
    {
        IReadOnlyList<SnapshotScore?> scores = SnapshotScorer.Score(ControlExample.Months());

        for (int i = 1; i < scores.Count; i++)
        {
            ChangeBreakdown change = scores[i]!.Change!;

            Assert.Equal(scores[i - 1]!.Index.FuzzyIndex, change.Previous, 1e-9);
            Assert.Equal(scores[i]!.Index.FuzzyIndex, change.Current, 1e-9);
        }
    }

    [Fact]
    public void SameAmounts_GiveTheSameResults()
    {
        SnapshotScore first = SnapshotScorer.Score(ControlExample.Months())[^1]!;
        SnapshotScore second = SnapshotScorer.Score(ControlExample.Months())[^1]!;

        Assert.Equal(first.Inputs, second.Inputs);
        Assert.Equal(first.Index.FuzzyIndex, second.Index.FuzzyIndex);
        Assert.Equal(first.Index.Linear, second.Index.Linear);
        Assert.Equal(first.Index.Final, second.Index.Final);
    }

    [Fact]
    public void ChangingAnEarlierSnapshot_ChangesTheFollowingOnes()
    {
        MonthFigures[] months = ControlExample.Months();
        SnapshotScore before = SnapshotScorer.Score(months)[^1]!;

        months[3] = months[3] with { Saved = 0 };
        SnapshotScore after = SnapshotScorer.Score(months)[^1]!;

        Assert.Equal(3.0 / 6, before.Inputs.Indicators.SavingsRegularity, 1e-9);
        Assert.Equal(2.0 / 6, after.Inputs.Indicators.SavingsRegularity, 1e-9);
        Assert.True(after.Index.FuzzyIndex < before.Index.FuzzyIndex);
    }
}
