using FluentValidation;
using FluentValidation.TestHelper;
using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Tests.Shared.Index.Indicators;

public sealed class MonthFiguresTests
{
    private static readonly MonthFiguresValidator Validator = new();

    private static readonly ExpenseBreakdown Expenses = new(10_000, 15_000, 0, 0, 0, 0, 5_000);

    [Fact]
    public void Actual_WithValidAmounts_IsCreated()
    {
        MonthFigures month = MonthFigures.Actual(60_000, Expenses, 5_000, 100_000, 10_000, 3);

        Validator.TestValidate(month).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Actual_RejectsASelfRatingOutsideOneToFive(int selfRating)
    {
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            MonthFigures.Actual(60_000, Expenses, 5_000, 100_000, 10_000, selfRating)
        );

        Assert.Equal(nameof(MonthFigures.SelfRating), Assert.Single(exception.Errors).PropertyName);
    }

    [Fact]
    public void Estimated_RejectsNegativeExpenses()
    {
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            MonthFigures.Estimated(-1, 0)
        );

        Assert.Equal(nameof(MonthFigures.Expenses), Assert.Single(exception.Errors).PropertyName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Validator_RejectsAmountsThatAreNegativeOrNotFinite(double amount)
    {
        MonthFigures month = new()
        {
            IsEstimated = false,
            Income = amount,
            Expenses = amount,
            Saved = amount,
            Reserves = amount,
            DebtPayments = amount,
        };

        TestValidationResult<MonthFigures> result = Validator.TestValidate(month);

        result.ShouldHaveValidationErrorFor(m => m.Income);
        result.ShouldHaveValidationErrorFor(m => m.Expenses);
        result.ShouldHaveValidationErrorFor(m => m.Saved);
        result.ShouldHaveValidationErrorFor(m => m.Reserves);
        result.ShouldHaveValidationErrorFor(m => m.DebtPayments);
    }
}
