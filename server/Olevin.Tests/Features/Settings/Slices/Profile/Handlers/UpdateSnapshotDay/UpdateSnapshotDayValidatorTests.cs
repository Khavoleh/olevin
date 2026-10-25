using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateSnapshotDay;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.UpdateSnapshotDay;

public sealed class UpdateSnapshotDayValidatorTests
{
    private readonly UpdateSnapshotDayValidator Validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(31)]
    public void SnapshotDay_WithinTheBounds_IsAccepted(short day) =>
        Validator
            .TestValidate(new UpdateSnapshotDayRequest(day))
            .ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    [InlineData(-1)]
    public void SnapshotDay_OutsideTheBounds_IsRejected(short day) =>
        Validator
            .TestValidate(new UpdateSnapshotDayRequest(day))
            .ShouldHaveValidationErrorFor(r => r.SnapshotDay);
}
