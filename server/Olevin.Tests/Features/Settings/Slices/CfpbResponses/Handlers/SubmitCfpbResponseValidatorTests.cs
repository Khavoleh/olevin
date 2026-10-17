using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Slices.CfpbResponses.Handlers;
using Olevin.Api.Features.Settings.Slices.CfpbResponses.Handlers.Enums;

namespace Olevin.Tests.Features.Settings.Slices.CfpbResponses.Handlers;

public sealed class SubmitCfpbResponseValidatorTests
{
    private readonly SubmitCfpbResponseValidator Validator = new();

    private static SubmitCfpbResponseRequest Valid() =>
        new(
            CfpbDescription.Completely,
            CfpbDescription.VeryWell,
            CfpbDescription.Somewhat,
            CfpbDescription.VeryLittle,
            CfpbDescription.NotAtAll,
            CfpbDescription.Somewhat,
            CfpbFrequency.Always,
            CfpbFrequency.Often,
            CfpbFrequency.Rarely,
            CfpbFrequency.Never,
            false
        );

    [Fact]
    public void ValidRequest_IsAccepted() =>
        Validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData(5)]
    [InlineData(-1)]
    public void UnknownDescription_IsRejected(int value) =>
        Validator
            .TestValidate(Valid() with { CouldHandleMajorExpense = (CfpbDescription)value })
            .ShouldHaveValidationErrorFor(r => r.CouldHandleMajorExpense);

    [Theory]
    [InlineData(5)]
    [InlineData(-1)]
    public void UnknownFrequency_IsRejected(int value) =>
        Validator
            .TestValidate(Valid() with { FinancesControlLife = (CfpbFrequency)value })
            .ShouldHaveValidationErrorFor(r => r.FinancesControlLife);
}
