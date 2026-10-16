using FluentValidation.TestHelper;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot.Requests;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot.Validators;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot.Validators;

public sealed class CreateFirstSnapshotValidatorTests
{
    private readonly CreateFirstSnapshotValidator Validator = new();

    private static CreateFirstSnapshotRequest Valid() =>
        new(
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
            4,
            new EstimateRequest(ControlExample.Uah(46_000), ControlExample.Uah(0)),
            new EstimateRequest(ControlExample.Uah(28_000), ControlExample.Uah(6_000))
        );

    [Fact]
    public void ValidRequest_IsAccepted() =>
        Validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void MissingIncome_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Income = null! })
            .ShouldHaveValidationErrorFor(r => r.Income);

    [Fact]
    public void MissingExpenses_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Expenses = null! })
            .ShouldHaveValidationErrorFor(r => r.Expenses);

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
    public void MissingExpenseCategory_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Expenses = Valid().Expenses with { Health = null! } })
            .ShouldHaveValidationErrorFor("Expenses.Health");

    [Fact]
    public void MissingDebtPayments_IsRejected() =>
        Validator
            .TestValidate(Valid() with { DebtPayments = null! })
            .ShouldHaveValidationErrorFor(r => r.DebtPayments);

    [Fact]
    public void MissingSaved_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Saved = null! })
            .ShouldHaveValidationErrorFor(r => r.Saved);

    [Fact]
    public void MissingReserves_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Reserves = null! })
            .ShouldHaveValidationErrorFor(r => r.Reserves);

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
    public void TooManyIncomeEntries_AreRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    Income = [.. Enumerable.Repeat(ControlExample.Uah(1), 11)],
                }
            )
            .ShouldHaveValidationErrorFor(r => r.Income);

    [Fact]
    public void TooManyDebtPayments_AreRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    DebtPayments = [.. Enumerable.Repeat(ControlExample.Uah(1), 11)],
                }
            )
            .ShouldHaveValidationErrorFor(r => r.DebtPayments);

    [Fact]
    public void TenIncomeEntries_AreAccepted() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    Income = [.. Enumerable.Repeat(ControlExample.Uah(1), 10)],
                }
            )
            .ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void SelfRating_OutsideTheBounds_IsRejected(int selfRating) =>
        Validator
            .TestValidate(Valid() with { SelfRating = selfRating })
            .ShouldHaveValidationErrorFor(r => r.SelfRating);

    [Fact]
    public void MissingEstimates_AreRejected()
    {
        TestValidationResult<CreateFirstSnapshotRequest> result = Validator.TestValidate(
            Valid() with
            {
                PreviousMonth = null!,
                TwoMonthsAgo = null!,
            }
        );

        result.ShouldHaveValidationErrorFor(r => r.PreviousMonth);
        result.ShouldHaveValidationErrorFor(r => r.TwoMonthsAgo);
    }

    [Fact]
    public void EstimateWithoutExpenses_IsRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    PreviousMonth = new EstimateRequest(null!, ControlExample.Uah(0)),
                }
            )
            .ShouldHaveValidationErrorFor("PreviousMonth.ExpensesTotal");

    [Fact]
    public void EstimateWithoutSaved_IsRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    TwoMonthsAgo = new EstimateRequest(ControlExample.Uah(1), null!),
                }
            )
            .ShouldHaveValidationErrorFor("TwoMonthsAgo.Saved");

    [Fact]
    public void EstimateWithUnsupportedCurrency_IsRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    PreviousMonth = new EstimateRequest(
                        new Money(1, (CurrencyCode)99),
                        ControlExample.Uah(0)
                    ),
                }
            )
            .ShouldHaveValidationErrorFor("PreviousMonth.ExpensesTotal.Currency");
}
