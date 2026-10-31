using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.ConfigureUserSettings;
using Olevin.Api.Shared;

namespace Olevin.Tests.Features.Settings.Slices.Onboarding.Handlers.ConfigureUserSettings;

public sealed class ConfigureUserSettingsValidatorTests
{
    private readonly ConfigureUserSettingsValidator Validator = new();

    private static ConfigureUserSettingsRequest Valid() =>
        new(CurrencyCode.UAH, LocaleCode.UK, 15, "Europe/Kyiv", true);

    [Fact]
    public void ValidRequest_IsAccepted() =>
        Validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void UnknownCurrency_IsRejected() =>
        Validator
            .TestValidate(Valid() with { BaseCurrency = (CurrencyCode)99 })
            .ShouldHaveValidationErrorFor(r => r.BaseCurrency);

    [Fact]
    public void UnknownLocale_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Locale = (LocaleCode)99 })
            .ShouldHaveValidationErrorFor(r => r.Locale);

    [Fact]
    public void SnapshotDay_OutsideTheBounds_IsRejected() =>
        Validator
            .TestValidate(Valid() with { SnapshotDay = 32 })
            .ShouldHaveValidationErrorFor(r => r.SnapshotDay);

    [Fact]
    public void UnknownTimezone_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Timezone = "Europe/Nowhere" })
            .ShouldHaveValidationErrorFor(r => r.Timezone);
}
