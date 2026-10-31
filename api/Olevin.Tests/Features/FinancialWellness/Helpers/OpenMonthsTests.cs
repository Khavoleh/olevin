using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Tests.Features.FinancialWellness.Helpers;

public sealed class OpenMonthsTests
{
    private const short SnapshotDay = 31;
    private static readonly SnapshotMonth Earliest = new(2026, CalendarMonth.August);
    private static readonly SnapshotMonth Latest = new(2026, CalendarMonth.October);

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

    [Fact]
    public void OpensOn_WithAFreeOpenMonth_IsNull() =>
        Assert.Null(
            OpenMonths.OpensOn(
                Earliest,
                Latest,
                [new SnapshotMonth(2026, CalendarMonth.August)],
                SnapshotDay
            )
        );

    [Fact]
    public void OpensOn_WithAMissedMonth_IsNull() =>
        Assert.Null(
            OpenMonths.OpensOn(
                Earliest,
                Latest,
                [
                    new SnapshotMonth(2026, CalendarMonth.August),
                    new SnapshotMonth(2026, CalendarMonth.October),
                ],
                SnapshotDay
            )
        );

    [Fact]
    public void OpensOn_WithEveryOpenMonthTaken_IsTheSnapshotDayOfTheNextMonth() =>
        Assert.Equal(
            new DateOnly(2026, 11, 30),
            OpenMonths.OpensOn(
                Earliest,
                Latest,
                [
                    new SnapshotMonth(2026, CalendarMonth.August),
                    new SnapshotMonth(2026, CalendarMonth.September),
                    new SnapshotMonth(2026, CalendarMonth.October),
                ],
                SnapshotDay
            )
        );

    [Fact]
    public void OpensOn_WithAFirstSnapshotBeforeItsDay_SkipsTheTakenMonth() =>
        Assert.Equal(
            new DateOnly(2026, 11, 30),
            OpenMonths.OpensOn(
                new SnapshotMonth(2026, CalendarMonth.October),
                new SnapshotMonth(2026, CalendarMonth.September),
                [new SnapshotMonth(2026, CalendarMonth.October)],
                SnapshotDay
            )
        );

    [Fact]
    public void Free_ListsTheMonthsWithoutASnapshot_TheLatestFirst() =>
        Assert.Equal(
            [
                new SnapshotMonth(2026, CalendarMonth.October),
                new SnapshotMonth(2026, CalendarMonth.August),
            ],
            OpenMonths.Free(Earliest, Latest, [new SnapshotMonth(2026, CalendarMonth.September)])
        );

    [Fact]
    public void Free_CrossesTheYear() =>
        Assert.Equal(
            [
                new SnapshotMonth(2026, CalendarMonth.January),
                new SnapshotMonth(2025, CalendarMonth.December),
            ],
            OpenMonths.Free(
                new SnapshotMonth(2025, CalendarMonth.November),
                new SnapshotMonth(2026, CalendarMonth.January),
                [new SnapshotMonth(2025, CalendarMonth.November)]
            )
        );

    [Fact]
    public void Free_WithEveryOpenMonthTaken_IsEmpty() =>
        Assert.Empty(
            OpenMonths.Free(
                Earliest,
                Latest,
                [
                    new SnapshotMonth(2026, CalendarMonth.August),
                    new SnapshotMonth(2026, CalendarMonth.September),
                    new SnapshotMonth(2026, CalendarMonth.October),
                ]
            )
        );

    [Fact]
    public void Free_WithAFirstSnapshotBeforeItsDay_IsEmpty() =>
        Assert.Empty(
            OpenMonths.Free(
                new SnapshotMonth(2026, CalendarMonth.October),
                new SnapshotMonth(2026, CalendarMonth.September),
                [new SnapshotMonth(2026, CalendarMonth.October)]
            )
        );
}
