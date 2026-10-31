namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.GetPushSubscriptions;

/// <summary>
/// A browser of the user that gets the push on the snapshot day.
/// </summary>
/// <param name="Id">The subscription identifier.</param>
/// <param name="Endpoint">The URL of the push service, which tells the current browser apart.</param>
/// <param name="DeviceName">The name of the browser and device.</param>
/// <param name="CreatedAt">When the push was turned on in the browser.</param>
/// <param name="UpdatedAt">When the browser last confirmed the subscription.</param>
public sealed record PushSubscriptionResponse(
    Guid Id,
    string Endpoint,
    string DeviceName,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
