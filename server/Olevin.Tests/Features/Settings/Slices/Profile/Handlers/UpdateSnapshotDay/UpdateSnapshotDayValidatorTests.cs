using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateSnapshotDay;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.UpdateSnapshotDay;

public sealed class UpdateSnapshotDayValidatorTests
{
    private readonly UpdateSnapshotDayValidator Validator = new();

    [Fact]
    public void DayOfTheMonth_IsAccepted() =>
        Validator.TestValidate(new UpdateSnapshotDayRequest(15)).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void SnapshotDay_OutsideTheBounds_IsRejected() =>
        Validator
            .TestValidate(new UpdateSnapshotDayRequest(32))
            .ShouldHaveValidationErrorFor(r => r.SnapshotDay);
}
