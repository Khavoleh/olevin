using Olevin.Api.Features.Settings.Data.UserSettings;

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
    /// When the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the profile was last changed.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The settings of the user.
    /// </summary>
    public UserSetting? Setting { get; set; }
}
