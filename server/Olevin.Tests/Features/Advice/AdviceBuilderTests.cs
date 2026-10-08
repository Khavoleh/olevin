using Olevin.Api.Features.Advice;
using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Tests.Features.Advice;

public sealed class AdviceBuilderTests
{
    [Fact]
    public void ProfileA_OnlyGetsAdviceOnRegularity()
    {
        // D = 40 000, E = 44 000, no debts: SR = −0.1; R = 6; CV = 0.05; saved once in six months.
        MonthFigures month = MonthFigures.Actual(
            40_000,
            new ExpenseBreakdown(12_000, 15_000, 2_000, 3_000, 4_000, 3_000, 5_000),
            0,
            264_000,
            0,
            3
        );
        CalculatedInputs inputs = new(
            new(-0.10, 6, 0, 0.05, 1.0 / 6),
            new(-0.10, 6, 0, 0.05, 1.0 / 6),
            6,
            44_000,
            44_000,
            2_200,
            1,
            6
        );

        IReadOnlyList<AdviceItem> advice = AdviceBuilder.Build(month, inputs);

        // SR stays fully "negative" after +0.05, so nothing changes.
        AdviceItem savings = advice.Single(a => a.Indicator == Indicator.SavingsRate);
        Assert.Equal(0, savings.FuzzyGain);
        Assert.Equal(AdviceRejection.NoGain, savings.Rejection);

        Assert.Equal(AdviceRejection.BestTerm, Rejection(advice, Indicator.Reserve));
        Assert.Equal(AdviceRejection.BestTerm, Rejection(advice, Indicator.DebtToIncome));
        Assert.Equal(AdviceRejection.BestTerm, Rejection(advice, Indicator.ExpenseVariation));

        AdviceItem regularity = advice.Single(a => a.IsShown);
        Assert.Equal(Indicator.SavingsRegularity, regularity.Indicator);
        Assert.Equal(1, regularity.Rank);
        Assert.True(regularity.FuzzyGain > 0);
    }

    [Fact]
    public void SavingsRate_WhenFlexibleExpensesAreNotEnough_CutsAllOfThemAndAnalysesTheSmallerStep()
    {
        // Wanted: 0.05 · 60 000 = 3 000; flexible: 500 + 500 + 500 + 0.1 · 10 000 = 2 500.
        MonthFigures month = MonthFigures.Actual(
            60_000,
            new ExpenseBreakdown(10_000, 36_500, 3_000, 500, 500, 3_000, 500),
            0,
            100_000,
            0,
            3
        );

        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);
        AdviceItem savings = AdviceBuilder
            .Build(month, inputs)
            .Single(a => a.Indicator == Indicator.SavingsRate);

        Assert.True(savings.IsPartial);
        Assert.Equal(2_500, savings.Amount);
        Assert.Equal(2_500.0 / 60_000, savings.Step, 1e-9);
        Assert.Equal(
            [
                ExpenseCategory.Food,
                ExpenseCategory.Clothing,
                ExpenseCategory.Restaurants,
                ExpenseCategory.Other,
            ],
            savings.Cuts.Select(cut => cut.Category)
        );
        Assert.Equal([1_000, 500, 500, 500], savings.Cuts.Select(cut => Math.Round(cut.Amount, 6)));

        SensitivityResult smaller = SensitivityAnalyzer.Analyze(
            inputs.Indicators,
            Indicator.SavingsRate,
            savings.Step
        );
        Assert.Equal(smaller.FuzzyGain, savings.FuzzyGain);
    }

    [Fact]
    public void SavingsRate_WithoutFlexibleExpenses_IsFilteredOut()
    {
        MonthFigures month = MonthFigures.Actual(
            40_000,
            new ExpenseBreakdown(0, 30_000, 2_000, 0, 0, 3_000, 0),
            0,
            100_000,
            0,
            3
        );

        AdviceItem savings = AdviceBuilder
            .Build(month, InputsCalculator.Calculate([month], 0))
            .Single(a => a.Indicator == Indicator.SavingsRate);

        Assert.True(savings.IsPartial);
        Assert.Equal(0, savings.Step);
        Assert.Empty(savings.Cuts);
        Assert.Equal(AdviceRejection.NoGain, savings.Rejection);
    }

    [Fact]
    public void Reserve_WithoutSurplus_IsFilteredOut()
    {
        MonthFigures month = MonthFigures.Actual(
            30_000,
            new ExpenseBreakdown(0, 0, 0, 0, 0, 0, 32_000),
            0,
            40_000,
            0,
            3
        );

        AdviceItem reserve = AdviceBuilder
            .Build(month, InputsCalculator.Calculate([month], 0))
            .Single(a => a.Indicator == Indicator.Reserve);

        Assert.Equal(0, reserve.Step);
        Assert.Equal(0, reserve.Amount);
        Assert.Equal(0, reserve.FuzzyGain);
        Assert.Equal(AdviceRejection.NoGain, reserve.Rejection);
    }

    [Fact]
    public void Reserve_Step_IsAtMostOneMonth()
    {
        MonthFigures month = MonthFigures.Actual(
            100_000,
            new ExpenseBreakdown(0, 0, 0, 0, 0, 0, 20_000),
            0,
            40_000,
            0,
            3
        );

        AdviceItem reserve = AdviceBuilder
            .Build(month, InputsCalculator.Calculate([month], 0))
            .Single(a => a.Indicator == Indicator.Reserve);

        Assert.Equal(1, reserve.Step);
        Assert.Equal(20_000, reserve.Amount);
    }

    [Fact]
    public void Ranks_AreConsecutiveAndOnlyForAdviceThatPassedTheFilter()
    {
        IReadOnlyList<AdviceItem> advice = ControlExampleAdvice.September();

        Assert.Equal([1, 2, 3, 4], advice.Select(a => a.Rank).OfType<int>().Order());
        Assert.Equal(3, advice.Count(a => a.IsShown));
    }

    [Fact]
    public void EqualGains_AreOrderedBySavingsRateReserveDebtVariationRegularity()
    {
        // D = 100 000, E = 70 000, no debts: SR = 0.3; R = 140 000 / 70 000 = 2; CV = 0.25. Moving R from 2 to 3 and
        // CV from 0.25 to 0.2 both turn a half-and-half input into one full middle term, so the gains are equal.
        MonthFigures month = MonthFigures.Actual(
            100_000,
            new ExpenseBreakdown(20_000, 20_000, 5_000, 5_000, 5_000, 5_000, 10_000),
            30_000,
            140_000,
            0,
            3
        );
        CalculatedInputs inputs = new(
            new(0.3, 2, 0, 0.25, 0.5),
            new(0.3, 2, 0, 0.25, 0.5),
            6,
            70_000,
            70_000,
            17_500,
            3,
            6
        );

        IReadOnlyList<AdviceItem> advice = AdviceBuilder.Build(month, inputs);

        AdviceItem reserve = advice.Single(a => a.Indicator == Indicator.Reserve);
        AdviceItem variation = advice.Single(a => a.Indicator == Indicator.ExpenseVariation);
        Assert.Equal(1, reserve.Step);
        Assert.True(reserve.FuzzyGain > 0);
        Assert.Equal(reserve.FuzzyGain, variation.FuzzyGain, AdviceConstants.Tolerance);
        Assert.Equal(1, reserve.Rank);
        Assert.Equal(2, variation.Rank);
    }

    private static AdviceRejection? Rejection(
        IReadOnlyList<AdviceItem> advice,
        Indicator indicator
    ) => advice.Single(a => a.Indicator == indicator).Rejection;
}
