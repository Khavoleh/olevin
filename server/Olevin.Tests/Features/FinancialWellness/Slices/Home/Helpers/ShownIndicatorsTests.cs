using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Helpers;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Home.Helpers;

public sealed class ShownIndicatorsTests
{
    private const double Precision = 1e-9;

    [Fact]
    public void ControlExample_KeepsTheValuesBeforeLimiting()
    {
        MonthFigures[] months = ControlExample.Months();
        CalculatedInputs inputs = InputsCalculator.Calculate(months, months.Length - 1);

        Assert.Equal(0.1, ShownIndicators.OfIncome(0.1, months[^1]));
        Assert.Equal(3.5, ShownIndicators.ReserveMonths(inputs, months[^1])!.Value, 0.05);
        Assert.Equal(0.2, ShownIndicators.ExpenseVariation(inputs)!.Value, 0.005);
    }

    [Fact]
    public void OfIncome_Deficit_IsNotLimited()
    {
        MonthFigures month = Actual(income: 40_000, expenses: 54_000, debtPayments: 0);
        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        Assert.Equal(
            -0.35,
            ShownIndicators.OfIncome(inputs.Raw.SavingsRate, month)!.Value,
            Precision
        );
    }

    [Fact]
    public void OfIncome_NoIncome_IsNull()
    {
        MonthFigures month = Actual(income: 0, expenses: 20_000, debtPayments: 5_000);

        Assert.Null(ShownIndicators.OfIncome(-0.2, month));
    }

    [Fact]
    public void ReserveMonths_NoExpensesOrDebtPayments_IsNull()
    {
        MonthFigures month = Actual(income: 40_000, expenses: 0, debtPayments: 0);
        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        Assert.Null(ShownIndicators.ReserveMonths(inputs, month));
    }

    [Fact]
    public void ReserveMonths_OnlyDebtPayments_IsTheReserveOverThem()
    {
        MonthFigures month = Actual(income: 40_000, expenses: 0, debtPayments: 10_000);
        CalculatedInputs inputs = InputsCalculator.Calculate([month], 0);

        Assert.Equal(12, ShownIndicators.ReserveMonths(inputs, month)!.Value, Precision);
    }

    [Fact]
    public void ExpenseVariation_FirstSnapshot_IsNull()
    {
        CalculatedInputs inputs = InputsCalculator.Calculate(ControlExample.Months()[^1..], 0);

        Assert.Null(ShownIndicators.ExpenseVariation(inputs));
    }

    private static MonthFigures Actual(double income, double expenses, double debtPayments) =>
        MonthFigures.Actual(
            income,
            new ExpenseBreakdown(0, 0, 0, 0, 0, 0, expenses),
            0,
            120_000,
            debtPayments,
            3
        );
}
