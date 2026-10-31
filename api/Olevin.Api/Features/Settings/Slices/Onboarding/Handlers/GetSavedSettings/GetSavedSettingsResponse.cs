using Olevin.Api.Shared;

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.GetSavedSettings;

/// <summary>
/// The settings saved during onboarding that the next steps depend on.
/// </summary>
/// <param name="BaseCurrency">The default currency.</param>
/// <param name="SnapshotDay">The day of the month (1-31) the monthly snapshot is taken on.</param>
/// <param name="Timezone">The IANA time zone name.</param>
public sealed record GetSavedSettingsResponse(
    CurrencyCode BaseCurrency,
    short SnapshotDay,
    string Timezone
);
