using FluentValidation.TestHelper;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Validators;

public sealed class EstimateRequestValidatorTests
{
    private readonly EstimateRequestValidator Validator = new();

    private static EstimateRequest Valid() =>
        new(ControlExample.Uah(46_000), ControlExample.Uah(0));

    [Fact]
    public void ValidRequest_IsAccepted() =>
        Validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void MissingExpenses_IsRejected() =>
        Validator
            .TestValidate(Valid() with { ExpensesTotal = null! })
            .ShouldHaveValidationErrorFor(r => r.ExpensesTotal);

    [Fact]
    public void MissingSaved_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Saved = null! })
            .ShouldHaveValidationErrorFor(r => r.Saved);

    [Fact]
    public void NegativeAmount_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Saved = ControlExample.Uah(-1) })
            .ShouldHaveValidationErrorFor("Saved.Amount");

    [Fact]
    public void AmountAboveTheLimit_IsRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    ExpensesTotal = new Money(1_000_000_000_001m, CurrencyCode.UAH),
                }
            )
            .ShouldHaveValidationErrorFor("ExpensesTotal.Amount");

    [Fact]
    public void UnsupportedCurrency_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Saved = new Money(1, (CurrencyCode)99) })
            .ShouldHaveValidationErrorFor("Saved.Currency");
}
