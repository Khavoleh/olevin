namespace Olevin.Api.Infrastructure.WebPush;

/// <summary>
/// The push services of the browsers, the only hosts the API sends Web Push messages to.
/// </summary>
public static class PushServiceHosts
{
    private static readonly string[] Hosts =
    [
        "fcm.googleapis.com",
        "updates.push.services.mozilla.com",
        "web.push.apple.com",
    ];

    private const string WindowsHostSuffix = ".notify.windows.com";

    /// <summary>
    /// Checks that a subscription endpoint is an HTTPS URL of a known push service.
    /// </summary>
    /// <param name="endpoint">The endpoint of a subscription.</param>
    /// <returns>Whether the API may send messages to the endpoint.</returns>
    public static bool IsKnown(string endpoint) =>
        Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri) && IsKnown(uri);

    /// <summary>
    /// Checks that a URL is an HTTPS URL of a known push service.
    /// </summary>
    /// <param name="uri">The URL.</param>
    /// <returns>Whether the URL belongs to a push service.</returns>
    public static bool IsKnown(Uri uri) =>
        uri.Scheme == Uri.UriSchemeHttps
        && (
            Hosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase)
            || uri.Host.EndsWith(WindowsHostSuffix, StringComparison.OrdinalIgnoreCase)
        );
}
