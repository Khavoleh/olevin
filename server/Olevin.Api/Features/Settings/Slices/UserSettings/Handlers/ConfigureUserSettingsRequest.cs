using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.Settings.Slices.UserSettings.Handlers;

/// <summary>
/// The settings of a user.
/// </summary>
/// <param name="BaseCurrency">The default currency.</param>
/// <param name="Locale">The interface language.</param>
/// <param name="SnapshotDay">The day of the month (1-31) the monthly snapshot is taken on.</param>
/// <param name="Timezone">The IANA time zone name.</param>
/// <param name="ResearchConsent">The user's research consent.</param>
public sealed record ConfigureUserSettingsRequest(
    CurrencyCode BaseCurrency,
    LocaleCode Locale,
    short SnapshotDay,
    string Timezone,
    bool ResearchConsent
);
