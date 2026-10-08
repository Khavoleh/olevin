using Olevin.Api.Features.Advice;
using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Tests.Features.Advice;

/// <summary>
/// The advice for profile Б: September of the control example.
/// </summary>
public sealed class ControlExampleAdviceTests
{
    private const double Precision = 0.1;

    private static readonly IReadOnlyList<AdviceItem> Advice = ControlExampleAdvice.September();

    [Fact]
    public void Advice_ShowsVariationDebtAndSavingsRate()
    {
        Assert.Equal(
            [Indicator.ExpenseVariation, Indicator.DebtToIncome, Indicator.SavingsRate],
            Advice.Where(a => a.IsShown).OrderBy(a => a.Rank).Select(a => a.Indicator)
        );
    }

    [Fact]
    public void Advice_HasAllFiveRowsWithTheReferenceGainsAndAmounts()
    {
        Assert.Equal(Enum.GetValues<Indicator>(), Advice.Select(a => a.Indicator));

        AssertRow(Indicator.ExpenseVariation, 7.6, 3.8, 1_750, rank: 1);
        AssertRow(Indicator.DebtToIncome, 6.3, 10.6, 3_000, rank: 2);
        AssertRow(Indicator.SavingsRate, 4.0, 6.3, 3_000, rank: 3);
        AssertRow(Indicator.Reserve, 2.5, 1.7, 18_000, rank: 4);
    }

    [Fact]
    public void Advice_Regularity_DoesNotRaiseTheIndex()
    {
        // With this cash flow and buffer "sometimes" and "regularly" lead to the same conclusions (rules 32/33 and
        // 35/36), so one more month with savings changes nothing in I_fuzzy, although I_lin grows.
        AdviceItem regularity = Row(Indicator.SavingsRegularity);

        Assert.Equal(0, regularity.FuzzyGain, 1e-9);
        Assert.Equal(5.0, regularity.LinearGain, Precision);
        Assert.Equal(AdviceRejection.NoGain, regularity.Rejection);
        Assert.Null(regularity.Rank);
    }

    [Fact]
    public void Advice_DebtStep_AlsoRaisesTheSavingsRate()
    {
        AdviceItem debt = Row(Indicator.DebtToIncome);

        Assert.Equal(-0.05, debt.Step, 1e-9);
        Assert.Equal(
            [Indicator.DebtToIncome, Indicator.SavingsRate],
            debt.Changes.Select(change => change.Indicator)
        );
        AssertChange(debt.Changes[0], 0.30, 0.25);
        AssertChange(debt.Changes[1], 0.10, 0.15);
    }

    [Fact]
    public void Advice_ReserveStep_IsWhatTheSurplusGivesOverThreeMonths()
    {
        AdviceItem reserve = Row(Indicator.Reserve);

        Assert.Equal(1.0 / 3, reserve.Step, 1e-9);
        AssertChange(reserve.Changes.Single(), 3.5, 3.5 + (1.0 / 3));
    }

    [Fact]
    public void Advice_RegularityStep_IsOneMoreMonthOfSix()
    {
        AdviceItem regularity = Row(Indicator.SavingsRegularity);

        Assert.Equal(1.0 / 6, regularity.Step, 1e-9);
        AssertChange(regularity.Changes.Single(), 3.0 / 6, 4.0 / 6);
    }

    [Fact]
    public void Advice_SavingsRateCut_SplitsBetweenFlexibleCategoriesInProportion()
    {
        AdviceItem savings = Row(Indicator.SavingsRate);

        Assert.False(savings.IsPartial);
        AssertChange(savings.Changes.Single(), 0.10, 0.15);
        Assert.Equal(
            [
                ExpenseCategory.Other,
                ExpenseCategory.Restaurants,
                ExpenseCategory.Clothing,
                ExpenseCategory.Food,
            ],
            savings.Cuts.Select(cut => cut.Category)
        );
        Assert.Equal([1_387, 756, 630, 227], savings.Cuts.Select(cut => Math.Round(cut.Amount)));
        Assert.Equal(3_000, savings.Cuts.Sum(cut => cut.Amount), 1e-6);
    }

    private static AdviceItem Row(Indicator indicator) =>
        Advice.Single(a => a.Indicator == indicator);

    private static void AssertRow(
        Indicator indicator,
        double fuzzyGain,
        double linearGain,
        double? amount,
        int rank
    )
    {
        AdviceItem row = Row(indicator);

        Assert.Equal(fuzzyGain, row.FuzzyGain, Precision);
        Assert.Equal(linearGain, row.LinearGain, Precision);
        Assert.Equal(amount, row.Amount is { } value ? Math.Round(value, 6) : null);
        Assert.Equal(rank, row.Rank);
        Assert.Equal(rank <= 3, row.IsShown);
    }

    private static void AssertChange(IndicatorChange change, double from, double to)
    {
        Assert.Equal(from, change.From, 1e-9);
        Assert.Equal(to, change.To, 1e-9);
    }
}
