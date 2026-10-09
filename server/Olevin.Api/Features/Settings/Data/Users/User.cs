using Olevin.Api.Features.Settings.Data.Users.Enums;

namespace Olevin.Api.Features.Settings.Data.Users;

/// <summary>
/// Application user profile, a row of <c>settings.users</c>.
/// </summary>
public sealed class User
{
    /// <summary>
    /// User identifier; a UUID v7 created by the application.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Logto sub claim.
    /// </summary>
    public string AuthSub { get; set; } = null!;

    /// <summary>
    /// Default user's currency.
    /// </summary>
    public CurrencyCode BaseCurrency { get; set; }

    /// <summary>
    /// Interface language.
    /// </summary>
    public LocaleCode Locale { get; set; }

    /// <summary>
    /// Day of the month the monthly snapshot is taken on (1-31).
    /// </summary>
    public short SnapshotDay { get; set; }

    /// <summary>
    /// IANA time zone name.
    /// </summary>
    public string Timezone { get; set; } = null!;

    /// <summary>
    /// When the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the profile was last changed.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
