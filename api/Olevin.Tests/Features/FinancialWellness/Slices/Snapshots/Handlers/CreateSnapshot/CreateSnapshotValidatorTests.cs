using FluentValidation.TestHelper;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;

public sealed class CreateSnapshotValidatorTests
{
    private readonly CreateSnapshotValidator Validator = new();

    private static CreateSnapshotRequest Valid() =>
        new(
            2026,
            CalendarMonth.September,
            [ControlExample.Uah(55_000), ControlExample.Uah(5_000)],
            new ExpensesRequest(
                ControlExample.Uah(9_000),
                ControlExample.Uah(12_000),
                ControlExample.Uah(0),
                ControlExample.Uah(0),
                ControlExample.Uah(0),
                ControlExample.Uah(0),
                ControlExample.Uah(5_500)
            ),
            ControlExample.Uah(4_000),
            [new Money(2_000, CurrencyCode.USD), ControlExample.Uah(106_000)],
            [ControlExample.Uah(0)],
            4
        );

    [Fact]
    public void ValidRequest_IsAccepted() =>
        Validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData(0)]
    [InlineData(1_999)]
    [InlineData(10_000)]
    public void Year_OutsideTheBounds_IsRejected(short year) =>
        Validator
            .TestValidate(Valid() with { Year = year })
            .ShouldHaveValidationErrorFor(r => r.Year);

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Month_NotACalendarMonth_IsRejected(int month) =>
        Validator
            .TestValidate(Valid() with { Month = (CalendarMonth)month })
            .ShouldHaveValidationErrorFor(r => r.Month);

    [Fact]
    public void MissingIncome_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Income = null! })
            .ShouldHaveValidationErrorFor(r => r.Income);

    [Fact]
    public void NegativeExpense_IsRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    Expenses = Valid().Expenses with { Food = ControlExample.Uah(-1) },
                }
            )
            .ShouldHaveValidationErrorFor("Expenses.Food.Amount");

    [Fact]
    public void MissingSaved_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Saved = null! })
            .ShouldHaveValidationErrorFor(r => r.Saved);

    [Fact]
    public void TooManyReserves_AreRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    Reserves = [.. Enumerable.Repeat(ControlExample.Uah(1), 11)],
                }
            )
            .ShouldHaveValidationErrorFor(r => r.Reserves);

    [Fact]
    public void InvalidDebtPayment_IsRejected() =>
        Validator
            .TestValidate(Valid() with { DebtPayments = [ControlExample.Uah(-5)] })
            .ShouldHaveValidationErrorFor("DebtPayments[0].Amount");

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void SelfRating_OutsideTheBounds_IsRejected(int rating) =>
        Validator
            .TestValidate(Valid() with { SelfRating = rating })
            .ShouldHaveValidationErrorFor(r => r.SelfRating);
}
