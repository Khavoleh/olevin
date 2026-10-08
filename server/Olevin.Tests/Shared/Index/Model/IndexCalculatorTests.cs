using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Tests.Shared.Index.Model;

public sealed class IndexCalculatorTests
{
    [Theory]
    [InlineData(1, 45)]
    [InlineData(2, 47.5)]
    [InlineData(3, 50)]
    [InlineData(4, 52.5)]
    [InlineData(5, 55)]
    public void SelfRating_MovesTheIndexBy2Point5PerStep(int selfRating, double expected) =>
        Assert.Equal(expected, IndexCalculator.AdjustForSelfRating(50, selfRating), 1e-9);

    [Fact]
    public void SelfRating_KeepsTheIndexWithin0And100()
    {
        Assert.Equal(0, IndexCalculator.AdjustForSelfRating(2, 1));
        Assert.Equal(100, IndexCalculator.AdjustForSelfRating(98, 5));
    }

    [Theory]
    [InlineData(0, Zone.Stress)]
    [InlineData(39.9, Zone.Stress)]
    [InlineData(40, Zone.Unstable)]
    [InlineData(69.9, Zone.Unstable)]
    [InlineData(70, Zone.Calm)]
    [InlineData(100, Zone.Calm)]
    public void Zones_SplitTheScaleAt40And70(double value, Zone zone) =>
        Assert.Equal(zone, Zones.Of(value));

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(7, false)]
    public void Index_IsPreliminaryWithFewerThanThreeActualSnapshots(
        int actualSnapshots,
        bool isPreliminary
    )
    {
        IndexResult result = IndexCalculator.Calculate(
            new IndicatorVector(0.1, 3.5, 0.3, 0.2, 0.5),
            3,
            actualSnapshots
        );

        Assert.Equal(isPreliminary, result.IsPreliminary);
    }
}
