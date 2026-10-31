using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Trend.Handlers;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Trend.Handlers;

public sealed class GetTrendRulesTests
{
    private static readonly SnapshotMonth October = new(2026, CalendarMonth.October);

    [Fact]
    public void Points_SixMonths_EndsAtTheLatestOpenMonth()
    {
        IReadOnlyList<TrendPointResponse> points = GetTrendRules.Points(
            TrendPeriod.SixMonths,
            October,
            [
                Snapshot(2025, CalendarMonth.December, 40),
                Snapshot(2026, CalendarMonth.May, 50),
                Snapshot(2026, CalendarMonth.October, 60),
            ]
        );

        Assert.Equal(
            [
                (2026, CalendarMonth.May),
                (2026, CalendarMonth.June),
                (2026, CalendarMonth.July),
                (2026, CalendarMonth.August),
                (2026, CalendarMonth.September),
                (2026, CalendarMonth.October),
            ],
            points.Select(point => ((int)point.Year, point.Month))
        );
    }

    [Fact]
    public void Points_TwelveMonths_CrossesTheYear()
    {
        IReadOnlyList<TrendPointResponse> points = GetTrendRules.Points(
            TrendPeriod.TwelveMonths,
            October,
            [Snapshot(2025, CalendarMonth.January, 40), Snapshot(2026, CalendarMonth.October, 60)]
        );

        Assert.Equal(12, points.Count);
        Assert.Equal((2025, CalendarMonth.November), (points[0].Year, points[0].Month));
        Assert.Equal((2026, CalendarMonth.October), (points[^1].Year, points[^1].Month));
    }

    [Fact]
    public void Points_All_StartsAtTheFirstSnapshot()
    {
        IReadOnlyList<TrendPointResponse> points = GetTrendRules.Points(
            TrendPeriod.All,
            October,
            [Snapshot(2026, CalendarMonth.October, 60), Snapshot(2025, CalendarMonth.March, 40)]
        );

        Assert.Equal(20, points.Count);
        Assert.Equal((2025, CalendarMonth.March), (points[0].Year, points[0].Month));
        Assert.Equal(40, points[0].Index);
    }

    [Fact]
    public void Points_PeriodBeforeTheFirstSnapshot_StartsAtTheFirstSnapshot()
    {
        IReadOnlyList<TrendPointResponse> points = GetTrendRules.Points(
            TrendPeriod.TwelveMonths,
            October,
            [Snapshot(2026, CalendarMonth.August, 50), Snapshot(2026, CalendarMonth.October, 60)]
        );

        Assert.Equal(
            [CalendarMonth.August, CalendarMonth.September, CalendarMonth.October],
            points.Select(point => point.Month)
        );
    }

    [Fact]
    public void Points_MonthWithoutSnapshot_HasNoValues()
    {
        IReadOnlyList<TrendPointResponse> points = GetTrendRules.Points(
            TrendPeriod.SixMonths,
            October,
            [Snapshot(2026, CalendarMonth.August, 50), Snapshot(2026, CalendarMonth.October, 60)]
        );

        TrendPointResponse september = points[1];

        Assert.Equal(CalendarMonth.September, september.Month);
        Assert.Null(september.Index);
        Assert.Null(september.CashFlow);
        Assert.Null(september.Buffer);
    }

    [Fact]
    public void Points_LatestOpenMonthWithoutSnapshot_EndsWithAGap()
    {
        IReadOnlyList<TrendPointResponse> points = GetTrendRules.Points(
            TrendPeriod.SixMonths,
            October,
            [Snapshot(2026, CalendarMonth.July, 50), Snapshot(2026, CalendarMonth.August, 55)]
        );

        Assert.Equal(CalendarMonth.October, points[^1].Month);
        Assert.Null(points[^1].Index);
        Assert.Null(points[^2].Index);
    }

    [Fact]
    public void Points_SnapshotAfterTheLatestOpenMonth_IsTheLastMonth()
    {
        IReadOnlyList<TrendPointResponse> points = GetTrendRules.Points(
            TrendPeriod.SixMonths,
            new SnapshotMonth(2026, CalendarMonth.September),
            [Snapshot(2026, CalendarMonth.October, 60)]
        );

        Assert.Equal(CalendarMonth.October, Assert.Single(points).Month);
    }

    [Fact]
    public void Points_NoSnapshotInThePeriod_IsEmpty() =>
        Assert.Empty(
            GetTrendRules.Points(
                TrendPeriod.SixMonths,
                October,
                [Snapshot(2026, CalendarMonth.March, 50), Snapshot(2026, CalendarMonth.April, 55)]
            )
        );

    [Fact]
    public void Points_OneSnapshot_IsOnePoint()
    {
        TrendPointResponse point = Assert.Single(
            GetTrendRules.Points(
                TrendPeriod.All,
                October,
                [Snapshot(2026, CalendarMonth.October, 47.25)]
            )
        );

        Assert.Equal(47.3, point.Index);
    }

    [Fact]
    public void Points_RoundsTheValuesToOneDecimal()
    {
        TrendPointResponse point = Assert.Single(
            GetTrendRules.Points(
                TrendPeriod.SixMonths,
                October,
                [new SnapshotDto(2026, CalendarMonth.October, 58.66, 49.96, 60.44)]
            )
        );

        Assert.Equal(58.7, point.Index);
        Assert.Equal(50.0, point.CashFlow);
        Assert.Equal(60.4, point.Buffer);
    }

    private static SnapshotDto Snapshot(short year, CalendarMonth month, double index) =>
        new(year, month, index, index, index);
}
