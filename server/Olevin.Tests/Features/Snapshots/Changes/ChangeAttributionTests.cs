using Olevin.Api.Features.Snapshots.Changes;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Tests.Features.Snapshots.Changes;

public sealed class ChangeAttributionTests
{
    private static readonly IndicatorVector ProfileB = new(0.10, 3.5, 0.30, 0.20, 0.5);

    [Fact]
    public void ContributionsAndJointEffect_AddUpToTheActualChange()
    {
        IndicatorVector current = new(0.18, 4.2, 0.22, 0.12, 4.0 / 6);

        ChangeBreakdown change = ChangeAttribution.Explain(ProfileB, current);

        Assert.Equal(OlevinModel.Evaluate(ProfileB).Value, change.Previous, 1e-9);
        Assert.Equal(OlevinModel.Evaluate(current).Value, change.Current, 1e-9);
        Assert.Equal(
            change.Total,
            change.Contributions.Sum(c => c.Contribution) + change.JointEffect,
            1e-9
        );
        Assert.Equal(Enum.GetValues<Indicator>(), change.Contributions.Select(c => c.Indicator));
    }

    [Fact]
    public void ChangeOfOneIndicator_IsAllItsOwn()
    {
        IndicatorVector current = ProfileB with { ExpenseVariation = 0.15 };

        ChangeBreakdown change = ChangeAttribution.Explain(ProfileB, current);

        Assert.Equal(7.6, change.Total, 0.1);
        Assert.Equal(change.Total, Contribution(change, Indicator.ExpenseVariation), 1e-9);
        Assert.Equal(0, change.JointEffect, 1e-9);
        Assert.All(
            change.Contributions.Where(c => c.Indicator != Indicator.ExpenseVariation),
            c => Assert.Equal(0, c.Contribution, 1e-9)
        );
    }

    [Fact]
    public void NoChange_HasNoContributions()
    {
        ChangeBreakdown change = ChangeAttribution.Explain(ProfileB, ProfileB);

        Assert.Equal(0, change.Total);
        Assert.All(change.Contributions, c => Assert.Equal(0, c.Contribution));
        Assert.Equal(0, change.JointEffect);
    }

    private static double Contribution(ChangeBreakdown change, Indicator indicator) =>
        change.Contributions.Single(c => c.Indicator == indicator).Contribution;
}
