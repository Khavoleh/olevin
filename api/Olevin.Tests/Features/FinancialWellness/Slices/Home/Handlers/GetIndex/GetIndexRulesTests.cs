using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetIndex;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Home.Handlers.GetIndex;

public sealed class GetIndexRulesTests
{
    private static readonly SnapshotMonth October = new(2026, CalendarMonth.October);

    [Fact]
    public void NextSnapshotDate_CurrentMonthIsFree_IsItsSnapshotDay() =>
        Assert.Equal(
            new DateOnly(2026, 10, 31),
            GetIndexRules.NextSnapshotDate(
                new SnapshotMonth(2026, CalendarMonth.September),
                October,
                31
            )
        );

    [Fact]
    public void NextSnapshotDate_AfterSkippedMonths_IsInTheCurrentMonth() =>
        Assert.Equal(
            new DateOnly(2026, 10, 5),
            GetIndexRules.NextSnapshotDate(new SnapshotMonth(2026, CalendarMonth.July), October, 5)
        );

    [Fact]
    public void NextSnapshotDate_CurrentMonthIsTaken_IsInTheNextMonth() =>
        Assert.Equal(
            new DateOnly(2026, 11, 15),
            GetIndexRules.NextSnapshotDate(October, October, 15)
        );

    [Fact]
    public void NextSnapshotDate_DayAfterTheEndOfTheMonth_IsItsLastDay() =>
        Assert.Equal(
            new DateOnly(2026, 11, 30),
            GetIndexRules.NextSnapshotDate(October, October, 31)
        );

    [Fact]
    public void NextSnapshotDate_DecemberIsTaken_IsInJanuaryOfTheNextYear()
    {
        SnapshotMonth december = new(2026, CalendarMonth.December);

        Assert.Equal(
            new DateOnly(2027, 1, 10),
            GetIndexRules.NextSnapshotDate(december, december, 10)
        );
    }
}
