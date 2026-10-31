using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Enums;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;
using Olevin.Tests.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices;

public sealed class GetAdvicesMappingTests
{
    private const double Precision = 1e-9;

    private static readonly CalculatedInputs Inputs = ControlExampleAdvice.SeptemberInputs();

    private static readonly IReadOnlyList<AdviceItem> Advice = ControlExampleAdvice.September();

    [Fact]
    public void ToResponses_ShowsTheTopThreeWithTheLargestGainFirst()
    {
        IReadOnlyList<AdviceResponse> shown = GetAdvicesMapping.ToResponses(Advice, Inputs);

        Assert.Collection(
            shown,
            advice => Assert.IsType<ExpenseVariationAdviceResponse>(advice),
            advice => Assert.IsType<DebtToIncomeAdviceResponse>(advice),
            advice => Assert.IsType<SavingsRateAdviceResponse>(advice)
        );
        Assert.Equal([7.6, 6.3, 4.0], shown.Select(advice => advice.Gain));
        Assert.Equal(AdviceStatus.Shown, GetAdvicesMapping.StatusOf(Advice, 0));
    }

    [Theory]
    [InlineData(3, AdviceStatus.AtMaximum)]
    [InlineData(1, AdviceStatus.AllBest)]
    public void StatusOf_WhenEveryIndicatorIsAtItsBest_DependsOnTheSelfRating(
        int selfRating,
        AdviceStatus status
    )
    {
        MonthFigures month = MonthFigures.Actual(
            100_000,
            new ExpenseBreakdown(10_000, 10_000, 1_000, 1_000, 1_000, 1_000, 1_000),
            50_000,
            400_000,
            0,
            selfRating
        );
        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        IReadOnlyList<AdviceItem> advice = AdviceBuilder.Build(month, inputs);

        Assert.Empty(GetAdvicesMapping.ToResponses(advice, inputs));
        Assert.Equal(status, GetAdvicesMapping.StatusOf(advice, FinalIndex(month, inputs)));
    }

    [Fact]
    public void StatusOf_WhenNoStepTheUserCanMakeRaisesTheIndex_IsNoQuickWin()
    {
        MonthFigures month = MonthFigures.Actual(
            30_000,
            new ExpenseBreakdown(8_000, 22_000, 1_000, 1_000, 0, 2_000, 1_000),
            0,
            0,
            0,
            3
        );
        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);
        IReadOnlyList<AdviceItem> advice = AdviceBuilder.Build(month, inputs);

        Assert.Empty(GetAdvicesMapping.ToResponses(advice, inputs));
        Assert.Equal(
            AdviceStatus.NoQuickWin,
            GetAdvicesMapping.StatusOf(advice, FinalIndex(month, inputs))
        );
    }

    [Fact]
    public void ToResponse_SavingsRate_GivesTheMonthlyCutByCategory()
    {
        SavingsRateAdviceResponse advice = Assert.IsType<SavingsRateAdviceResponse>(
            Map(Indicator.SavingsRate)
        );

        Assert.Equal(3_000, advice.Amount);
        Assert.False(advice.IsPartial);
        AssertChange(advice.SavingsRate, 0.10, 0.15);
        Assert.Equal(
            [
                ExpenseCategory.Other,
                ExpenseCategory.Restaurants,
                ExpenseCategory.Clothing,
                ExpenseCategory.Food,
            ],
            advice.Cuts.Select(cut => cut.Category)
        );
        Assert.All(advice.Cuts, cut => Assert.Equal(Math.Round(cut.Amount, 2), cut.Amount));
    }

    [Fact]
    public void ToResponse_Reserve_SplitsTheTopUpIntoMonths()
    {
        ReserveAdviceResponse advice = Assert.IsType<ReserveAdviceResponse>(Map(Indicator.Reserve));

        Assert.Equal(2.5, advice.Gain);
        Assert.Equal(6_000, advice.MonthlyAmount);
        Assert.Equal(3, advice.Months);
        AssertChange(advice.Reserve, 3.5, 3.5 + (1.0 / 3));
    }

    [Fact]
    public void ToResponse_DebtToIncome_AlsoGivesTheSavingsRate()
    {
        DebtToIncomeAdviceResponse advice = Assert.IsType<DebtToIncomeAdviceResponse>(
            Map(Indicator.DebtToIncome)
        );

        Assert.Equal(3_000, advice.Amount);
        AssertChange(advice.DebtToIncome, 0.30, 0.25);
        AssertChange(advice.SavingsRate, 0.10, 0.15);
    }

    [Fact]
    public void ToResponse_ExpenseVariation_GivesTheCutOfTheDeviation()
    {
        ExpenseVariationAdviceResponse advice = Assert.IsType<ExpenseVariationAdviceResponse>(
            Map(Indicator.ExpenseVariation)
        );

        Assert.Equal(1_750, advice.Amount);
        Assert.Equal(-0.05, advice.ExpenseVariation.To - advice.ExpenseVariation.From, Precision);
    }

    [Fact]
    public void ToResponse_SavingsRegularity_GivesTheMonthsWithSavings()
    {
        SavingsRegularityAdviceResponse advice = Assert.IsType<SavingsRegularityAdviceResponse>(
            Map(Indicator.SavingsRegularity)
        );

        Assert.Equal(0, advice.Gain);
        Assert.Equal(3, advice.SavingMonths);
        Assert.Equal(6, advice.Months);
    }

    private static double FinalIndex(MonthFigures month, CalculatedInputs inputs) =>
        IndexCalculator.AdjustForSelfRating(
            OlevinModel.Evaluate(inputs.Indicators).Value,
            month.SelfRating
        );

    private static AdviceResponse Map(Indicator indicator) =>
        GetAdvicesMapping.ToResponse(Advice.Single(item => item.Indicator == indicator), Inputs);

    private static void AssertChange(IndicatorChangeResponse change, double from, double to)
    {
        Assert.Equal(from, change.From, Precision);
        Assert.Equal(to, change.To, Precision);
    }
}
