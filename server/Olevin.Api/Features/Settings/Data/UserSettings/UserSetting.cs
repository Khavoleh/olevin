using Olevin.Api.Features.Settings.Data.Users;
using Olevin.Api.Features.Settings.Data.UserSettings.Enums;

namespace Olevin.Api.Features.Settings.Data.UserSettings;

/// <summary>
/// Per-user application settings, a row of <c>settings.user_settings</c>.
/// </summary>
public sealed class UserSetting
{
    /// <summary>
    /// Owning user identifier.
    /// </summary>
    public Guid UserId { get; set; }

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
    /// When the settings were created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the settings were last changed.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The owning user.
    /// </summary>
    public User User { get; set; } = null!;
}
