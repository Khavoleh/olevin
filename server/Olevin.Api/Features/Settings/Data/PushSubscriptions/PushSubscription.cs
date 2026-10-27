using Olevin.Api.Features.Settings.Data.Users;

namespace Olevin.Api.Features.Settings.Data.PushSubscriptions;

/// <summary>
/// Web Push subscription of a user's browser or device, a row of <c>settings.push_subscriptions</c>.
/// </summary>
public sealed class PushSubscription
{
    /// <summary>
    /// Subscription identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Owning user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// URL of the push service the messages are sent to.
    /// </summary>
    public string Endpoint { get; set; } = null!;

    /// <summary>
    /// Public key of the browser the payload is encrypted for.
    /// </summary>
    public string P256dh { get; set; } = null!;

    /// <summary>
    /// Authentication secret of the browser.
    /// </summary>
    public string Auth { get; set; } = null!;

    /// <summary>
    /// Name of the browser and device shown to the user.
    /// </summary>
    public string DeviceName { get; set; } = null!;

    /// <summary>
    /// Year of the snapshot day push last sent to the subscription.
    /// </summary>
    public short? SnapshotPushYear { get; set; }

    /// <summary>
    /// Month of the snapshot day push last sent to the subscription.
    /// </summary>
    public CalendarMonth? SnapshotPushMonth { get; set; }

    /// <summary>
    /// When the subscription was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the browser last confirmed the subscription.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The owning user.
    /// </summary>
    public User User { get; set; } = null!;
}
