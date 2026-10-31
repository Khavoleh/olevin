using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateLocale;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.UpdateLocale;

public sealed class UpdateLocaleValidatorTests
{
    private readonly UpdateLocaleValidator Validator = new();

    [Fact]
    public void SupportedLocale_IsAccepted() =>
        Validator
            .TestValidate(new UpdateLocaleRequest(LocaleCode.EN))
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void UnknownLocale_IsRejected() =>
        Validator
            .TestValidate(new UpdateLocaleRequest((LocaleCode)99))
            .ShouldHaveValidationErrorFor(r => r.Locale);
}
