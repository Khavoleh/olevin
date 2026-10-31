using FluentValidation;
using Olevin.Api.Features.Settings.Validators;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateTimezone;

/// <summary>
/// Validates the new time zone of a user.
/// </summary>
public sealed class UpdateTimezoneValidator : AbstractValidator<UpdateTimezoneRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTimezoneValidator"/> class.
    /// </summary>
    public UpdateTimezoneValidator()
    {
        RuleFor(request => request.Timezone).Timezone();
    }
}
