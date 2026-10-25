using FluentValidation;
using Olevin.Api.Features.Settings.Validators;

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.ConfigureUserSettings;

/// <summary>
/// Validates the settings of a user.
/// </summary>
public sealed class ConfigureUserSettingsValidator : AbstractValidator<ConfigureUserSettingsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureUserSettingsValidator"/> class.
    /// </summary>
    public ConfigureUserSettingsValidator()
    {
        RuleFor(request => request.BaseCurrency).BaseCurrency();

        RuleFor(request => request.Locale).Locale();

        RuleFor(request => request.SnapshotDay).SnapshotDay();

        RuleFor(request => request.Timezone).Timezone();
    }
}
