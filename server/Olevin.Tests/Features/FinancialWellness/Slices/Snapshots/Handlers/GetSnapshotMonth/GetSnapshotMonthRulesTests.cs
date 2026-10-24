using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshotMonth;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshotMonth;

public sealed class GetSnapshotMonthRulesTests
{
    private static readonly SnapshotMonth[] Snapshots =
    [
        new(2025, CalendarMonth.December),
        new(2026, CalendarMonth.January),
        new(2026, CalendarMonth.March),
    ];

    [Theory]
    [InlineData(2025, CalendarMonth.November, 3)]
    [InlineData(2025, CalendarMonth.December, 2)]
    [InlineData(2026, CalendarMonth.February, 1)]
    [InlineData(2026, CalendarMonth.March, 0)]
    public void CountLater_CountsOnlyTheLaterMonths(
        short year,
        CalendarMonth month,
        int expected
    ) =>
        Assert.Equal(
            expected,
            GetSnapshotMonthRules.CountLater(new SnapshotMonth(year, month), Snapshots)
        );
}
