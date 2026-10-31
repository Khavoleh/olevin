using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Enums;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Responses;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Home.Handlers.GetComposition;

public sealed class GetCompositionRulesTests
{
    [Theory]
    [InlineData(0, SubsystemTerm.Weak)]
    [InlineData(24.9, SubsystemTerm.Weak)]
    [InlineData(25, SubsystemTerm.Weak)]
    [InlineData(50, SubsystemTerm.Moderate)]
    [InlineData(60.4, SubsystemTerm.Moderate)]
    [InlineData(75, SubsystemTerm.Moderate)]
    [InlineData(75.1, SubsystemTerm.Strong)]
    [InlineData(100, SubsystemTerm.Strong)]
    public void SubsystemTermOf_IsTheTermWithTheHighestDegree(double value, SubsystemTerm term) =>
        Assert.Equal(term, GetCompositionRules.SubsystemTermOf(value));

    [Theory]
    [InlineData(0, 6, SavingsRegularityTerm.Rarely)]
    [InlineData(2, 6, SavingsRegularityTerm.Rarely)]
    [InlineData(1, 5, SavingsRegularityTerm.Rarely)]
    [InlineData(3, 6, SavingsRegularityTerm.Sometimes)]
    [InlineData(4, 6, SavingsRegularityTerm.Regularly)]
    [InlineData(1, 1, SavingsRegularityTerm.Regularly)]
    public void SavingsRegularityTermOf_IsTheTermWithTheHighestDegree(
        int savingMonths,
        int months,
        SavingsRegularityTerm term
    ) =>
        Assert.Equal(
            term,
            GetCompositionRules.SavingsRegularityTermOf((double)savingMonths / months)
        );

    [Fact]
    public void SavingsMonths_ControlExample_AreAprilToSeptemberWithTheMonthsSaved()
    {
        IReadOnlyList<SavingsMonthResponse> months = GetCompositionRules.SavingsMonths(
            ControlExample.Snapshots()
        );

        Assert.Equal(
            [
                CalendarMonth.April,
                CalendarMonth.May,
                CalendarMonth.June,
                CalendarMonth.July,
                CalendarMonth.August,
                CalendarMonth.September,
            ],
            months.Select(month => month.Month)
        );
        Assert.Equal(
            [true, false, false, true, false, true],
            months.Select(month => month.HasSaved)
        );
    }

    [Fact]
    public void SavingsMonths_FirstSnapshot_IsOneMonth()
    {
        IReadOnlyList<SavingsMonthResponse> months = GetCompositionRules.SavingsMonths(
            ControlExample.Snapshots()[^1..]
        );

        SavingsMonthResponse month = Assert.Single(months);
        Assert.Equal(new SavingsMonthResponse(2026, CalendarMonth.September, true), month);
    }

    [Fact]
    public void SavingsMonths_LongHistory_AreTheLastSixWithEstimates()
    {
        SnapshotFigures[] history =
        [
            Estimate(CalendarMonth.February, 1_000),
            Estimate(CalendarMonth.March, 0),
            .. ControlExample.Snapshots(),
        ];

        IReadOnlyList<SavingsMonthResponse> months = GetCompositionRules.SavingsMonths(history);

        Assert.Equal(6, months.Count);
        Assert.Equal(CalendarMonth.April, months[0].Month);
        Assert.Equal(CalendarMonth.September, months[^1].Month);
    }

    [Fact]
    public void SavingsMonths_EstimateWithoutSavings_HasNotSaved()
    {
        IReadOnlyList<SavingsMonthResponse> months = GetCompositionRules.SavingsMonths([
            Estimate(CalendarMonth.August, 0),
            .. ControlExample.Snapshots()[^1..],
        ]);

        Assert.Equal([false, true], months.Select(month => month.HasSaved));
    }

    private static SnapshotFigures Estimate(CalendarMonth month, double saved) =>
        new(new SnapshotMonth(2026, month), MonthFigures.Estimated(30_000, saved));
}
