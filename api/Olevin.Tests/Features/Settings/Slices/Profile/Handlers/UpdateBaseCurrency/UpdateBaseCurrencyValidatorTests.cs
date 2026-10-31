using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateBaseCurrency;
using Olevin.Api.Shared;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.UpdateBaseCurrency;

public sealed class UpdateBaseCurrencyValidatorTests
{
    private readonly UpdateBaseCurrencyValidator Validator = new();

    [Fact]
    public void SupportedCurrency_IsAccepted() =>
        Validator
            .TestValidate(new UpdateBaseCurrencyRequest(CurrencyCode.EUR))
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void UnknownCurrency_IsRejected() =>
        Validator
            .TestValidate(new UpdateBaseCurrencyRequest((CurrencyCode)99))
            .ShouldHaveValidationErrorFor(r => r.BaseCurrency);
}
