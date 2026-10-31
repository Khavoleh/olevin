using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Enums;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;

public sealed class AdviceBuilderTests
{
    private const double Precision = 1e-9;

    private static readonly MonthFigures[] Estimates =
    [
        MonthFigures.Estimated(30_000, 3_000),
        MonthFigures.Estimated(32_000, 0),
    ];

    [Fact]
    public void ProfileA_GetsALargerCutOfExpensesAndAdviceOnRegularity()
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

        AdviceItem savings = advice.Single(a => a.Indicator == Indicator.SavingsRate);
        Assert.Equal(0.12, savings.Step, Precision);
        Assert.Equal(4_800, savings.Amount!.Value, Precision);
        AssertChange(savings.Changes.Single(), -0.1, 0.02);
        Assert.True(savings.FuzzyGain >= AdviceConstants.ExtendedGain);
        Assert.Equal(1, savings.Rank);

        Assert.Equal(AdviceRejection.BestTerm, Rejection(advice, Indicator.Reserve));
        Assert.Equal(AdviceRejection.BestTerm, Rejection(advice, Indicator.DebtToIncome));
        Assert.Equal(AdviceRejection.BestTerm, Rejection(advice, Indicator.ExpenseVariation));

        AdviceItem regularity = advice.Single(a => a.Indicator == Indicator.SavingsRegularity);
        Assert.Equal(2, regularity.Rank);
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

    [Fact]
    public void SavingsRate_InADeficit_CutsJustEnoughForFivePoints()
    {
        IReadOnlyList<AdviceItem> advice = BuildLatest(Month(30_000, other: 12_000));

        AdviceItem savings = advice.Single(a => a.Indicator == Indicator.SavingsRate);
        Assert.Null(savings.Rejection);
        Assert.Equal(1, savings.Rank);
        Assert.Equal(0.3, savings.Step, Precision);
        Assert.Equal(9_000, savings.Amount!.Value, Precision);
        Assert.False(savings.IsPartial);
        Assert.Equal(9_000, savings.Cuts.Sum(cut => cut.Amount), Precision);
        AssertChange(savings.Changes.Single(), -0.2, 0.1);
        Assert.InRange(savings.FuzzyGain, AdviceConstants.ExtendedGain, 6);
    }

    [Fact]
    public void SavingsRate_InADeepDeficit_StartsFromTheRawValue()
    {
        AdviceItem savings = BuildLatest(Month(30_000, other: 21_000))
            .Single(a => a.Indicator == Indicator.SavingsRate);

        Assert.Equal(0.6, savings.Step, Precision);
        Assert.Equal(18_000, savings.Amount!.Value, Precision);
        AssertChange(savings.Changes.Single(), -0.5, 0.1);
    }

    [Fact]
    public void SavingsRate_WhenFivePointsAreOutOfReach_TakesTheLargestGainWithinReach()
    {
        AdviceItem savings = BuildLatest(Month(30_000, other: 3_700, housing: 18_300))
            .Single(a => a.Indicator == Indicator.SavingsRate);

        Assert.Null(savings.Rejection);
        Assert.Equal(0.25, savings.Step, Precision);
        Assert.Equal(7_500, savings.Amount!.Value, Precision);
        Assert.InRange(savings.FuzzyGain, AdviceConstants.Tolerance, AdviceConstants.ExtendedGain);
    }

    [Fact]
    public void SavingsRate_InADeficitWithoutEnoughFlexibleExpenses_IsFilteredOut()
    {
        IReadOnlyList<AdviceItem> advice = BuildLatest(
            Month(30_000, other: 1_000, housing: 22_000, restaurants: 0)
        );

        Assert.Equal(AdviceRejection.NoGain, Rejection(advice, Indicator.SavingsRate));
        Assert.DoesNotContain(advice, a => a.IsShown);
    }

    [Fact]
    public void DebtToIncome_WithHighDebtAndADeficit_CutsMoreOfThePayments()
    {
        AdviceItem debt = BuildLatest(Month(40_000, other: 2_000, debts: 18_000))
            .Single(a => a.Indicator == Indicator.DebtToIncome);

        Assert.Null(debt.Rejection);
        Assert.Equal(-0.2, debt.Step, Precision);
        Assert.Equal(8_000, debt.Amount!.Value, Precision);
        AssertChange(debt.Changes[0], 0.45, 0.25);
        AssertChange(debt.Changes[1], -0.1, 0.1);
        Assert.True(debt.FuzzyGain >= AdviceConstants.ExtendedGain);
    }

    [Fact]
    public void SavingsRateAndDebt_WithoutIncome_AreNotExtended()
    {
        IReadOnlyList<AdviceItem> advice = BuildLatest(Month(0, other: 2_000, debts: 3_000));

        Assert.Equal(AdviceRejection.NoGain, Rejection(advice, Indicator.SavingsRate));
        Assert.Equal(AdviceRejection.NoGain, Rejection(advice, Indicator.DebtToIncome));
    }

    [Fact]
    public void FinalGain_NearTheTop_IsWhatIsLeftUpTo100()
    {
        (MonthFigures month, CalculatedInputs inputs) = NearTheTop(regularity: 4.0 / 6);
        double current = FinalIndex(month, inputs);

        AdviceItem regularity = AdviceBuilder
            .Build(month, inputs)
            .Single(a => a.Indicator == Indicator.SavingsRegularity);

        Assert.InRange(current, 90, 99);
        Assert.Null(regularity.Rejection);
        Assert.Equal(100 - current, regularity.FinalGain, Precision);
        Assert.True(regularity.FinalGain < regularity.FuzzyGain);
    }

    [Fact]
    public void FinalGain_AtTheTop_FiltersTheAdviceOut()
    {
        (MonthFigures month, CalculatedInputs inputs) = NearTheTop(regularity: 0.79);

        IReadOnlyList<AdviceItem> advice = AdviceBuilder.Build(month, inputs);
        AdviceItem regularity = advice.Single(a => a.Indicator == Indicator.SavingsRegularity);

        Assert.Equal(100, FinalIndex(month, inputs), Precision);
        Assert.True(regularity.FuzzyGain > 0);
        Assert.Equal(0, regularity.FinalGain, Precision);
        Assert.Equal(AdviceRejection.NoGain, regularity.Rejection);
        Assert.DoesNotContain(advice, a => a.IsShown);
    }

    private static (MonthFigures Month, CalculatedInputs Inputs) NearTheTop(double regularity)
    {
        MonthFigures month = MonthFigures.Actual(
            100_000,
            new ExpenseBreakdown(20_000, 20_000, 5_000, 5_000, 5_000, 5_000, 10_000),
            30_000,
            560_000,
            0,
            5
        );
        IndicatorVector indicators = new(0.3, 8, 0, 0, regularity);

        return (month, new CalculatedInputs(indicators, indicators, 6, 70_000, 70_000, 0, 4, 6));
    }

    private static double FinalIndex(MonthFigures month, CalculatedInputs inputs) =>
        IndexCalculator.AdjustForSelfRating(
            OlevinModel.Evaluate(inputs.Indicators).Value,
            month.SelfRating
        );

    private static MonthFigures Month(
        double income,
        double other,
        double debts = 0,
        double housing = 10_000,
        double restaurants = 2_000
    ) =>
        MonthFigures.Actual(
            income,
            new ExpenseBreakdown(8_000, housing, 1_000, 1_000, restaurants, 2_000, other),
            0,
            0,
            debts,
            3
        );

    private static IReadOnlyList<AdviceItem> BuildLatest(MonthFigures month)
    {
        MonthFigures[] months = [.. Estimates, month];

        return AdviceBuilder.Build(month, InputsCalculator.Calculate(months, months.Length - 1));
    }

    private static void AssertChange(IndicatorChange change, double from, double to)
    {
        Assert.Equal(from, change.From, Precision);
        Assert.Equal(to, change.To, Precision);
    }

    private static AdviceRejection? Rejection(
        IReadOnlyList<AdviceItem> advice,
        Indicator indicator
    ) => advice.Single(a => a.Indicator == indicator).Rejection;
}
