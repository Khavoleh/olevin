using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Helpers;

public sealed class OpenMonthsTests
{
    [Fact]
    public void Earliest_IsTheMonthAfterTheLatestEstimate() =>
        Assert.Equal(
            new SnapshotMonth(2026, CalendarMonth.January),
            OpenMonths.Earliest([
                new SnapshotMonth(2025, CalendarMonth.December),
                new SnapshotMonth(2025, CalendarMonth.November),
            ])
        );

    [Theory]
    [InlineData(2026, 10, 15, 15, 2026, CalendarMonth.October)]
    [InlineData(2026, 10, 20, 15, 2026, CalendarMonth.October)]
    [InlineData(2026, 10, 14, 15, 2026, CalendarMonth.September)]
    [InlineData(2026, 1, 14, 15, 2025, CalendarMonth.December)]
    public void Latest_OpensTheCurrentMonthOnItsSnapshotDay(
        int year,
        int month,
        int day,
        short snapshotDay,
        short expectedYear,
        CalendarMonth expectedMonth
    ) =>
        Assert.Equal(
            new SnapshotMonth(expectedYear, expectedMonth),
            OpenMonths.Latest(new DateOnly(year, month, day), snapshotDay)
        );

    [Theory]
    [InlineData(10, 30, CalendarMonth.September)]
    [InlineData(10, 31, CalendarMonth.October)]
    [InlineData(9, 29, CalendarMonth.August)]
    [InlineData(9, 30, CalendarMonth.September)]
    [InlineData(2, 28, CalendarMonth.February)]
    public void Latest_OnASnapshotDayAfterTheEndOfTheMonth_OpensOnTheLastDay(
        int month,
        int day,
        CalendarMonth expected
    ) =>
        Assert.Equal(
            new SnapshotMonth(2026, expected),
            OpenMonths.Latest(new DateOnly(2026, month, day), 31)
        );
}
