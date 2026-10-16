using Olevin.Api.Shared;

namespace Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;

/// <summary>
/// The user with their settings.
/// </summary>
/// <param name="UserId">The user identifier.</param>
/// <param name="BaseCurrency">The user's base currency.</param>
/// <param name="SnapshotDay">The day of the month (1-31) the monthly snapshot is taken on.</param>
/// <param name="Timezone">The IANA time zone name of the user.</param>
public sealed record UserSettingsDto(
    Guid UserId,
    CurrencyCode BaseCurrency,
    short SnapshotDay,
    string Timezone
);
