using Olevin.Api.Features.Settings.Data.Users.Enums;

namespace Olevin.Api.Features.Settings.Handlers.CreateUser;

/// <summary>
/// The settings of a new user.
/// </summary>
/// <param name="BaseCurrency">The default currency.</param>
/// <param name="Locale">The interface language.</param>
/// <param name="SnapshotDay">The day of the month (1-31) the monthly snapshot is taken on.</param>
/// <param name="Timezone">The IANA time zone name.</param>
public sealed record CreateUserRequest(
    CurrencyCode BaseCurrency,
    LocaleCode Locale,
    short SnapshotDay,
    string Timezone
);
