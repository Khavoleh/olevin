using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions.Responses;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Home.Handlers.GetContributions;

public sealed class GetContributionsRulesTests
{
    private const double Precision = 1e-9;

    private static readonly MonthFigures[] Months = ControlExample.Months();

    private static readonly CalculatedInputs August = InputsCalculator.Calculate(Months, 4);

    private static readonly CalculatedInputs September = InputsCalculator.Calculate(Months, 5);

    private static readonly double Change = IndexPoints.Delta(
        IndexCalculator.Calculate(September.Indicators, 4, 6).Final,
        IndexCalculator.Calculate(August.Indicators, 3, 5).Final
    );

    private static readonly GetContributionsResponse Contributions =
        GetContributionsRules.Contributions(Months[4], August, Months[5], September, Change);

    [Fact]
    public void PreviousSnapshot_SkipsEstimates()
    {
        MonthFigures estimate = MonthFigures.Estimated(30_000, 0);

        Assert.Equal(0, GetContributionsRules.PreviousSnapshot([Months[4], estimate, Months[5]]));
    }

    [Fact]
    public void PreviousSnapshot_FirstSnapshotAfterEstimates_IsNone()
    {
        MonthFigures estimate = MonthFigures.Estimated(30_000, 0);

        Assert.Equal(-1, GetContributionsRules.PreviousSnapshot([estimate, estimate, Months[5]]));
    }

    [Fact]
    public void IndicatorPoints_ControlExample_AddUpToTheChangeOfTheFuzzyIndex()
    {
        IReadOnlyDictionary<Indicator, double> points = GetContributionsRules.IndicatorPoints(
            August.Indicators,
            September.Indicators
        );

        Assert.Equal(
            OlevinModel.Evaluate(September.Indicators).Value
                - OlevinModel.Evaluate(August.Indicators).Value,
            points.Values.Sum(),
            Precision
        );
    }

    [Fact]
    public void IndicatorPoints_OneChangedIndicator_GetsTheWholeChange()
    {
        IndicatorVector latest = August.Indicators with { Reserve = 7 };

        IReadOnlyDictionary<Indicator, double> points = GetContributionsRules.IndicatorPoints(
            August.Indicators,
            latest
        );

        Assert.Equal(
            OlevinModel.Evaluate(latest).Value - OlevinModel.Evaluate(August.Indicators).Value,
            points[Indicator.Reserve],
            Precision
        );
        Assert.All(
            points.Where(item => item.Key != Indicator.Reserve),
            item => Assert.Equal(0, item.Value, Precision)
        );
    }

    [Theory]
    [InlineData(50, 3, 50, 4, 2.5)]
    [InlineData(50, 5, 50, 1, -10)]
    [InlineData(98, 3, 98, 5, 2)]
    public void SelfRatingPoints_IsTheDifferenceOfTheCorrectionsWithinTheScale(
        double previousFuzzy,
        int previousRating,
        double latestFuzzy,
        int latestRating,
        double points
    ) =>
        Assert.Equal(
            points,
            GetContributionsRules.SelfRatingPoints(
                previousFuzzy,
                previousRating,
                latestFuzzy,
                latestRating
            ),
            Precision
        );

    [Fact]
    public void RoundToTotal_RoundedValuesFallShort_MovesTheClosestUp() =>
        Assert.Equal([1.1, 1.0, 1.0], GetContributionsRules.RoundToTotal([1.04, 1.04, 1.04], 3.1));

    [Fact]
    public void RoundToTotal_RoundedValuesGoOver_MovesTheClosestDown() =>
        Assert.Equal([1.0, 1.1], GetContributionsRules.RoundToTotal([1.06, 1.06], 2.1));

    [Fact]
    public void RoundToTotal_RoundedValuesMatch_KeepsThem() =>
        Assert.Equal([0.3, -0.3], GetContributionsRules.RoundToTotal([0.26, -0.26], 0));

    [Fact]
    public void Contributions_ControlExample_AddUpToTheChangeOfTheShownIndex() =>
        Assert.Equal(
            Change,
            IndexPoints.Round(
                Contributions.SavingsRate.Points
                    + Contributions.Reserve.Points
                    + Contributions.DebtToIncome.Points
                    + Contributions.ExpenseVariation.Points
                    + Contributions.SavingsRegularity.Points
                    + Contributions.SelfRating.Points
            ),
            Precision
        );

    [Fact]
    public void Contributions_ControlExample_ShowTheValuesOfBothSnapshots()
    {
        Assert.Equal(0, Contributions.SavingsRate.From!.Value, Precision);
        Assert.Equal(0.1, Contributions.SavingsRate.To!.Value, Precision);
        Assert.Equal(0.3, Contributions.DebtToIncome.To!.Value, Precision);
        Assert.Equal(3.5, Contributions.Reserve.To!.Value, Precision);
        Assert.Equal(0.2, Contributions.ExpenseVariation.To!.Value, Precision);
        Assert.Equal(
            new SavingsRegularityContributionResponse(
                Contributions.SavingsRegularity.Points,
                2,
                5,
                3,
                6
            ),
            Contributions.SavingsRegularity
        );
        Assert.Equal(new SelfRatingContributionResponse(2.5, 3, 4), Contributions.SelfRating);
    }

    [Fact]
    public void Contributions_ControlExample_RiseWithEveryImprovedIndicator()
    {
        Assert.Equal(0, Contributions.DebtToIncome.Points, Precision);
        Assert.True(Contributions.SavingsRate.Points > 0);
        Assert.True(Contributions.SavingsRegularity.Points > 0);
        Assert.True(Contributions.Reserve.Points > 0);
        Assert.True(Contributions.ExpenseVariation.Points > 0);
    }
}
