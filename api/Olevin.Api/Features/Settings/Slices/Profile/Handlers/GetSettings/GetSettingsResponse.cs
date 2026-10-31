using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.GetSettings;

/// <summary>
/// The settings of the signed-in user shown on the profile page.
/// </summary>
/// <param name="Username">The Logto username, or <see langword="null"/> when the user has none.</param>
/// <param name="Email">The primary email, or <see langword="null"/> when the user has none.</param>
/// <param name="CreatedAt">When the user was created.</param>
/// <param name="BaseCurrency">The default currency.</param>
/// <param name="Locale">The saved interface language.</param>
/// <param name="SnapshotDay">The day of the month (1-31) the monthly snapshot is taken on.</param>
/// <param name="Timezone">The IANA time zone name.</param>
/// <param name="ResearchConsent">Whether the user consented to use of their data for research.</param>
/// <param name="PushNotifications">Whether any browser of the user gets the push on the snapshot day.</param>
public sealed record GetSettingsResponse(
    string? Username,
    string? Email,
    DateTime CreatedAt,
    CurrencyCode BaseCurrency,
    LocaleCode Locale,
    short SnapshotDay,
    string Timezone,
    bool ResearchConsent,
    bool PushNotifications
);
