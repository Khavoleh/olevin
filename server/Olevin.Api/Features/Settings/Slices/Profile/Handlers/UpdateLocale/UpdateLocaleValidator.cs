using FluentValidation;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateLocale;

/// <summary>
/// Validates the new interface language of a user.
/// </summary>
public sealed class UpdateLocaleValidator : AbstractValidator<UpdateLocaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLocaleValidator"/> class.
    /// </summary>
    public UpdateLocaleValidator()
    {
        RuleFor(request => request.Locale)
            .NotNull()
            .IsInEnum()
            .WithMessage("Locale is not supported.");
    }
}
