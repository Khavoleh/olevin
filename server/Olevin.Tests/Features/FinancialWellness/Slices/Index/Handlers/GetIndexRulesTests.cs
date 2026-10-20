using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Index.Handlers;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Index.Handlers;

public sealed class GetIndexRulesTests
{
    private const double Precision = 1e-9;

    private static readonly SnapshotMonth October = new(2026, CalendarMonth.October);

    [Theory]
    [InlineData(58.66, 58.7)]
    [InlineData(58.64, 58.6)]
    public void Round_KeepsOneDecimal(double index, double expected) =>
        Assert.Equal(expected, GetIndexRules.Round(index), Precision);

    [Fact]
    public void Delta_IsTheDifferenceOfTheRoundedIndexes() =>
        Assert.Equal(3.6, GetIndexRules.Delta(58.74, 55.06), Precision);

    [Fact]
    public void Delta_IndexFell_IsNegative() =>
        Assert.Equal(-2.5, GetIndexRules.Delta(52.6, 55.1), Precision);

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
