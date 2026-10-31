using FluentValidation;
using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Features.Settings.Validators;
using Olevin.Api.Shared;

namespace Olevin.Tests.Features.Settings.Validators;

public sealed class UserSettingRulesTests
{
    private readonly SettingsValidator Validator = new();

    private static SettingsRequest Valid() =>
        new(CurrencyCode.UAH, LocaleCode.UK, 15, "Europe/Kyiv");

    [Fact]
    public void ValidSettings_AreAccepted() =>
        Validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData(CurrencyCode.USD)]
    [InlineData(CurrencyCode.EUR)]
    [InlineData(CurrencyCode.CAD)]
    [InlineData(CurrencyCode.UAH)]
    [InlineData(CurrencyCode.PLN)]
    public void SupportedCurrency_IsAccepted(CurrencyCode currency) =>
        Validator
            .TestValidate(Valid() with { BaseCurrency = currency })
            .ShouldNotHaveValidationErrorFor(r => r.BaseCurrency);

    [Fact]
    public void UnknownCurrency_IsRejected() =>
        Validator
            .TestValidate(Valid() with { BaseCurrency = (CurrencyCode)99 })
            .ShouldHaveValidationErrorFor(r => r.BaseCurrency);

    [Theory]
    [InlineData(LocaleCode.UK)]
    [InlineData(LocaleCode.EN)]
    public void SupportedLocale_IsAccepted(LocaleCode locale) =>
        Validator
            .TestValidate(Valid() with { Locale = locale })
            .ShouldNotHaveValidationErrorFor(r => r.Locale);

    [Fact]
    public void UnknownLocale_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Locale = (LocaleCode)99 })
            .ShouldHaveValidationErrorFor(r => r.Locale);

    [Theory]
    [InlineData(1)]
    [InlineData(31)]
    public void SnapshotDay_AtTheBounds_IsAccepted(short day) =>
        Validator
            .TestValidate(Valid() with { SnapshotDay = day })
            .ShouldNotHaveValidationErrorFor(r => r.SnapshotDay);

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    [InlineData(-1)]
    public void SnapshotDay_OutsideTheBounds_IsRejected(short day) =>
        Validator
            .TestValidate(Valid() with { SnapshotDay = day })
            .ShouldHaveValidationErrorFor(r => r.SnapshotDay);

    [Theory]
    [InlineData("UTC")]
    [InlineData("Europe/Kyiv")]
    [InlineData("America/Argentina/Buenos_Aires")]
    [InlineData("Etc/GMT+3")]
    public void Timezone_IanaName_IsAccepted(string timezone) =>
        Validator
            .TestValidate(Valid() with { Timezone = timezone })
            .ShouldNotHaveValidationErrorFor(r => r.Timezone);

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Europe/")]
    [InlineData("/Kyiv")]
    [InlineData("Europe//Kyiv")]
    [InlineData("Europe/Kyiv ")]
    [InlineData("Europe/Kyiv\n")]
    [InlineData("Europe/Zürich")]
    public void Timezone_NotAnIanaName_IsRejected(string timezone) =>
        Validator
            .TestValidate(Valid() with { Timezone = timezone })
            .ShouldHaveValidationErrorFor(r => r.Timezone);

    [Fact]
    public void Timezone_WellFormedButUnknown_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Timezone = "Europe/Nowhere" })
            .ShouldHaveValidationErrorFor(r => r.Timezone);

    [Fact]
    public void Timezone_LongerThanTheColumn_IsRejected() =>
        Validator
            .TestValidate(Valid() with { Timezone = new string('a', 65) })
            .ShouldHaveValidationErrorFor(r => r.Timezone);

    private sealed record SettingsRequest(
        CurrencyCode BaseCurrency,
        LocaleCode Locale,
        short SnapshotDay,
        string Timezone
    );

    private sealed class SettingsValidator : AbstractValidator<SettingsRequest>
    {
        public SettingsValidator()
        {
            RuleFor(request => request.BaseCurrency).BaseCurrency();
            RuleFor(request => request.Locale).Locale();
            RuleFor(request => request.SnapshotDay).SnapshotDay();
            RuleFor(request => request.Timezone).Timezone();
        }
    }
}
