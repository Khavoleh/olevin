using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Data.Users.Enums;
using Olevin.Api.Features.Settings.Handlers.CreateUser;

namespace Olevin.Tests.Features.Settings.Handlers.CreateUser;

public sealed class CreateUserValidatorTests
{
    private readonly CreateUserValidator Validator = new();

    private static CreateUserRequest Valid() =>
        new(CurrencyCode.UAH, LocaleCode.UK, 15, "Europe/Kyiv");

    [Fact]
    public void ValidRequest_IsAccepted() =>
        Validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

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
    [InlineData("Europe/Київ")]
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
}
