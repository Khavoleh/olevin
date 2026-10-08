using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Tests.Shared.Index.Indicators;

public sealed class InputsCalculatorTests
{
    private const double Precision = 1e-9;

    private static readonly ExpenseBreakdown Spending = new(0, 0, 0, 0, 0, 0, 30_000);

    [Fact]
    public void SavingsRate_IsTheIncomeLeftAfterExpensesAndPayments_NotTheSavedField()
    {
        MonthFigures month = MonthFigures.Actual(
            50_000,
            Spending,
            saved: 0,
            0,
            debtPayments: 5_000,
            3
        );

        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        Assert.Equal(0.3, inputs.Indicators.SavingsRate, Precision);
        Assert.Equal(0.1, inputs.Indicators.DebtToIncome, Precision);
    }

    [Fact]
    public void Reserve_CoversAverageExpensesOfThreeMonthsPlusCurrentPayments()
    {
        MonthFigures[] months =
        [
            MonthFigures.Estimated(10_000, 0),
            MonthFigures.Estimated(20_000, 0),
            MonthFigures.Estimated(30_000, 0),
            MonthFigures.Actual(
                60_000,
                new ExpenseBreakdown(0, 0, 0, 0, 0, 0, 40_000),
                0,
                70_000,
                5_000,
                3
            ),
        ];

        CalculatedInputs inputs = InputsCalculator.Calculate(months, 3);

        Assert.Equal(30_000, inputs.AverageExpenses, Precision);
        Assert.Equal(2, inputs.Indicators.Reserve, Precision);
    }

    [Fact]
    public void VariationAndRegularity_UseTheLastSixSnapshots()
    {
        double[] expenses = [100_000, 100_000, 10_000, 10_000, 10_000, 10_000, 10_000, 10_000];
        MonthFigures[] months =
        [
            .. expenses[..^1].Select((e, i) => MonthFigures.Estimated(e, i < 2 ? 1 : 0)),
            MonthFigures.Actual(
                30_000,
                new ExpenseBreakdown(0, 0, 0, 0, 0, 0, expenses[^1]),
                500,
                0,
                0,
                3
            ),
        ];

        CalculatedInputs inputs = InputsCalculator.Calculate(months, months.Length - 1);

        Assert.Equal(6, inputs.HistoryMonths);
        Assert.Equal(0, inputs.Indicators.ExpenseVariation, Precision);
        Assert.Equal(1, inputs.SavingMonths);
        Assert.Equal(1.0 / 6, inputs.Indicators.SavingsRegularity, Precision);
    }

    [Fact]
    public void Variation_DividesTheDeviationByM()
    {
        MonthFigures[] months =
        [
            MonthFigures.Estimated(30_000, 0),
            MonthFigures.Actual(50_000, new ExpenseBreakdown(0, 0, 0, 0, 0, 0, 50_000), 0, 0, 0, 3),
        ];

        CalculatedInputs inputs = InputsCalculator.Calculate(months, 1);

        // μ = 40 000, σ = 10 000 (not 14 142 with m − 1).
        Assert.Equal(10_000, inputs.ExpenseDeviation, Precision);
        Assert.Equal(0.25, inputs.Indicators.ExpenseVariation, Precision);
    }

    [Fact]
    public void ColdStart_TwoEstimatesAndOneActualSnapshot_GiveThreePoints()
    {
        MonthFigures[] months =
        [
            MonthFigures.Estimated(28_000, 2_000),
            MonthFigures.Estimated(32_000, 0),
            MonthFigures.Actual(
                50_000,
                new ExpenseBreakdown(0, 0, 0, 0, 0, 0, 30_000),
                3_000,
                60_000,
                0,
                3
            ),
        ];

        CalculatedInputs inputs = InputsCalculator.Calculate(months, 2);

        Assert.Equal(3, inputs.AvailableSnapshots);
        Assert.Equal(3, inputs.HistoryMonths);
        Assert.Equal(30_000, inputs.AverageExpenses, Precision);
        Assert.Equal(2.0 / 3, inputs.Indicators.SavingsRegularity, Precision);
        Assert.Equal(2, inputs.Indicators.Reserve, Precision);
    }

    [Fact]
    public void SingleSnapshot_HasNoVariation()
    {
        MonthFigures month = MonthFigures.Actual(50_000, Spending, 1_000, 0, 0, 3);

        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        Assert.Equal(0, inputs.Indicators.ExpenseVariation);
        Assert.Equal(1, inputs.Indicators.SavingsRegularity);
    }

    [Theory]
    [InlineData(5_000, 0.6)]
    [InlineData(0, 0)]
    public void NoIncome_GivesTheWorstSavingsRate_AndDebtToIncomeByPayments(
        double payments,
        double debtToIncome
    )
    {
        MonthFigures month = MonthFigures.Actual(0, Spending, 0, 10_000, payments, 3);

        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        Assert.Equal(-0.2, inputs.Indicators.SavingsRate, Precision);
        Assert.Equal(debtToIncome, inputs.Indicators.DebtToIncome, Precision);
    }

    [Theory]
    [InlineData(10_000, 9)]
    [InlineData(0, 0)]
    public void NoExpensesAndPayments_GiveTheReserveByReserves(double reserves, double reserve)
    {
        MonthFigures month = MonthFigures.Actual(
            50_000,
            new ExpenseBreakdown(0, 0, 0, 0, 0, 0, 0),
            0,
            reserves,
            0,
            3
        );

        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        Assert.Equal(reserve, inputs.Indicators.Reserve, Precision);
        Assert.Equal(0, inputs.Indicators.ExpenseVariation, Precision);
    }

    [Fact]
    public void Indicators_AreClampedToTheirRanges_WhileRawValuesAreKept()
    {
        MonthFigures month = MonthFigures.Actual(
            40_000,
            new ExpenseBreakdown(0, 0, 0, 0, 0, 0, 40_000),
            0,
            1_000_000,
            14_000,
            3
        );

        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        Assert.Equal(-0.35, inputs.Raw.SavingsRate, Precision);
        Assert.Equal(-0.2, inputs.Indicators.SavingsRate, Precision);
        Assert.Equal(9, inputs.Indicators.Reserve, Precision);
        Assert.True(inputs.Raw.Reserve > 9);
    }
}
