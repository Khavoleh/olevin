using Olevin.Api.Features.Settings.Data.UserSettings;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateLocale;

/// <summary>
/// The new interface language of a user.
/// </summary>
/// <param name="Locale">The interface language.</param>
public sealed record UpdateLocaleRequest(LocaleCode Locale);
