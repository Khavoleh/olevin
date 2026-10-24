using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.History.Handlers.DeleteSnapshot;

namespace Olevin.Tests.Features.FinancialWellness.Slices.History.Handlers.DeleteSnapshot;

public sealed class DeleteSnapshotRulesTests
{
    private static readonly SnapshotMonth[] Remaining =
    [
        new(2025, CalendarMonth.December),
        new(2026, CalendarMonth.July),
    ];

    [Theory]
    [InlineData(2026, CalendarMonth.August)]
    [InlineData(2027, CalendarMonth.January)]
    public void IsLatest_AfterEveryRemainingMonth_IsTrue(short year, CalendarMonth month) =>
        Assert.True(DeleteSnapshotRules.IsLatest(new SnapshotMonth(year, month), Remaining));

    [Theory]
    [InlineData(2026, CalendarMonth.June)]
    [InlineData(2025, CalendarMonth.November)]
    public void IsLatest_BeforeARemainingMonth_IsFalse(short year, CalendarMonth month) =>
        Assert.False(DeleteSnapshotRules.IsLatest(new SnapshotMonth(year, month), Remaining));

    [Fact]
    public void IsLatest_ComparesTheYearBeforeTheMonth() =>
        Assert.True(
            DeleteSnapshotRules.IsLatest(
                new SnapshotMonth(2026, CalendarMonth.January),
                [new SnapshotMonth(2025, CalendarMonth.December)]
            )
        );
}
