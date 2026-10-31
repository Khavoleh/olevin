using FluentValidation;
using Olevin.Api.Features.Settings.Validators;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateBaseCurrency;

/// <summary>
/// Validates the new base currency of a user.
/// </summary>
public sealed class UpdateBaseCurrencyValidator : AbstractValidator<UpdateBaseCurrencyRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBaseCurrencyValidator"/> class.
    /// </summary>
    public UpdateBaseCurrencyValidator()
    {
        RuleFor(request => request.BaseCurrency).BaseCurrency();
    }
}
