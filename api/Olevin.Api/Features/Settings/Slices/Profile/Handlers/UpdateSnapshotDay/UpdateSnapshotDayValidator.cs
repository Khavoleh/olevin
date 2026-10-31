using FluentValidation;
using Olevin.Api.Features.Settings.Validators;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateSnapshotDay;

/// <summary>
/// Validates the new snapshot day of a user.
/// </summary>
public sealed class UpdateSnapshotDayValidator : AbstractValidator<UpdateSnapshotDayRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSnapshotDayValidator"/> class.
    /// </summary>
    public UpdateSnapshotDayValidator()
    {
        RuleFor(request => request.SnapshotDay).SnapshotDay();
    }
}
