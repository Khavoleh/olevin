using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Models;

public sealed class SnapshotMonthTests
{
    [Fact]
    public void Current_IsTheMonthInTheUsersTimezone()
    {
        // 23:30 on 30 September in UTC is already 1 October in Kyiv.
        DateTimeOffset now = new(2026, 9, 30, 23, 30, 0, TimeSpan.Zero);

        Assert.Equal(
            new SnapshotMonth(2026, CalendarMonth.October),
            SnapshotMonth.Current(now, "Europe/Kyiv")
        );
        Assert.Equal(
            new SnapshotMonth(2026, CalendarMonth.September),
            SnapshotMonth.Current(now, "UTC")
        );
    }

    [Fact]
    public void Previous_OfJanuary_IsDecemberOfThePreviousYear() =>
        Assert.Equal(
            new SnapshotMonth(2025, CalendarMonth.December),
            new SnapshotMonth(2026, CalendarMonth.January).Previous()
        );

    [Fact]
    public void Previous_InTheMiddleOfTheYear_KeepsTheYear() =>
        Assert.Equal(
            new SnapshotMonth(2026, CalendarMonth.September),
            new SnapshotMonth(2026, CalendarMonth.October).Previous()
        );

    [Fact]
    public void Next_OfDecember_IsJanuaryOfTheNextYear() =>
        Assert.Equal(
            new SnapshotMonth(2027, CalendarMonth.January),
            new SnapshotMonth(2026, CalendarMonth.December).Next()
        );

    [Fact]
    public void Next_InTheMiddleOfTheYear_KeepsTheYear() =>
        Assert.Equal(
            new SnapshotMonth(2026, CalendarMonth.November),
            new SnapshotMonth(2026, CalendarMonth.October).Next()
        );

    [Theory]
    [InlineData(2026, CalendarMonth.October, 15, 15)]
    [InlineData(2026, CalendarMonth.February, 31, 28)]
    [InlineData(2028, CalendarMonth.February, 31, 29)]
    [InlineData(2026, CalendarMonth.April, 31, 30)]
    public void DateOn_IsTheSnapshotDayOrTheLastDayOfTheMonth(
        short year,
        CalendarMonth month,
        short snapshotDay,
        int expectedDay
    ) =>
        Assert.Equal(
            new DateOnly(year, (int)month, expectedDay),
            new SnapshotMonth(year, month).DateOn(snapshotDay)
        );
}
