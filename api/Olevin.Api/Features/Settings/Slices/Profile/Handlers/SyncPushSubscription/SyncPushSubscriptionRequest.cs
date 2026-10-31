namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.SyncPushSubscription;

/// <summary>
/// The push subscription a browser still has.
/// </summary>
/// <param name="Endpoint">The URL of the push service from the subscription.</param>
public sealed record SyncPushSubscriptionRequest(string Endpoint);
