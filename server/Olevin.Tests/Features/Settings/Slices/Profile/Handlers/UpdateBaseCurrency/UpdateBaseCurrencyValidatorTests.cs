using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateBaseCurrency;
using Olevin.Api.Shared;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.UpdateBaseCurrency;

public sealed class UpdateBaseCurrencyValidatorTests
{
    private readonly UpdateBaseCurrencyValidator Validator = new();

    [Theory]
    [InlineData(CurrencyCode.USD)]
    [InlineData(CurrencyCode.EUR)]
    [InlineData(CurrencyCode.CAD)]
    [InlineData(CurrencyCode.UAH)]
    [InlineData(CurrencyCode.PLN)]
    public void SupportedCurrency_IsAccepted(CurrencyCode currency) =>
        Validator
            .TestValidate(new UpdateBaseCurrencyRequest(currency))
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void UnknownCurrency_IsRejected() =>
        Validator
            .TestValidate(new UpdateBaseCurrencyRequest((CurrencyCode)99))
            .ShouldHaveValidationErrorFor(r => r.BaseCurrency);
}
