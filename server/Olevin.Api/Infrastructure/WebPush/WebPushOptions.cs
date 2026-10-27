namespace Olevin.Api.Infrastructure.WebPush;

/// <summary>
/// Settings of the VAPID key pair the API signs Web Push messages with.
/// </summary>
public sealed class WebPushOptions
{
    /// <summary>
    /// The configuration section that holds these settings.
    /// </summary>
    public const string SectionName = "WebPush";

    /// <summary>
    /// Gets the contact of the sender for the push services, a <c>mailto:</c> or an <c>https:</c> URL.
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// Gets the public key of the pair (base64url), which the browsers subscribe with.
    /// </summary>
    public required string PublicKey { get; init; }

    /// <summary>
    /// Gets the private key of the pair (base64url).
    /// </summary>
    public required string PrivateKey { get; init; }
}
