namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.SubscribePush;

/// <summary>
/// The push subscription of the browser the user turns the push on in.
/// </summary>
/// <param name="Endpoint">The URL of the push service from the subscription.</param>
/// <param name="P256dh">The <c>p256dh</c> key from the subscription (base64url).</param>
/// <param name="Auth">The <c>auth</c> key from the subscription (base64url).</param>
/// <param name="DeviceName">The name of the browser and device shown to the user.</param>
public sealed record SubscribePushRequest(
    string Endpoint,
    string P256dh,
    string Auth,
    string DeviceName
);
