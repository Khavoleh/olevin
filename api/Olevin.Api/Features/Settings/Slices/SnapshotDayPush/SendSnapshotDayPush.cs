namespace Olevin.Api.Features.Settings.Slices.SnapshotDayPush;

/// <summary>
/// Asks to send the push of a snapshot day to one browser.
/// </summary>
/// <param name="SubscriptionId">The identifier of the push subscription of the browser.</param>
/// <param name="SnapshotDay">The snapshot day in the user's time zone.</param>
public sealed record SendSnapshotDayPush(Guid SubscriptionId, DateOnly SnapshotDay);
