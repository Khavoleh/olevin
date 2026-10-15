using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Helpers;

public sealed class FxRateLookupTests
{
    private static readonly DateOnly Date = new(2026, 10, 15);

    [Fact]
    public void ExactDate_IsTaken() =>
        Assert.Equal(Date, FxRateLookup.Closest(Date, [Date, Date.AddDays(1)]));

    [Fact]
    public void CloserDate_IsTaken() =>
        Assert.Equal(
            Date.AddDays(2),
            FxRateLookup.Closest(Date, [Date.AddDays(-3), Date.AddDays(2)])
        );

    [Fact]
    public void Tie_TakesTheEarlierDate() =>
        Assert.Equal(
            Date.AddDays(-2),
            FxRateLookup.Closest(Date, [Date.AddDays(-2), Date.AddDays(2)])
        );

    [Fact]
    public void DateBeforeAllRates_TakesTheEarliestRates() =>
        Assert.Equal(Date.AddDays(40), FxRateLookup.Closest(Date, [Date.AddDays(40)]));

    [Fact]
    public void DateAfterAllRates_TakesTheLatestRates() =>
        Assert.Equal(Date.AddDays(-5), FxRateLookup.Closest(Date, [Date.AddDays(-5)]));
}
