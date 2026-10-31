using System.Diagnostics;

namespace Olevin.Api.Infrastructure.WebPush;

/// <summary>
/// Hides the subscription endpoints, which are secret URLs of the browsers, from traces.
/// </summary>
public static class WebPushRedaction
{
    private const string Redacted = "REDACTED";

    /// <summary>
    /// The span tags that may hold the request URL, in the legacy and the stable HTTP semantic conventions.
    /// </summary>
    private static readonly string[] UrlTags = ["url.full", "url.path", "http.url", "http.target"];

    /// <summary>
    /// Replaces the URL tags of an HTTP client span with the host of the push service. Meant for the
    /// <c>EnrichWithHttpRequestMessage</c> option of the HTTP client instrumentation.
    /// </summary>
    /// <param name="activity">The span of the request.</param>
    /// <param name="request">The request.</param>
    public static void RedactSpan(Activity activity, HttpRequestMessage request)
    {
        if (request.RequestUri is not { } uri || !PushServiceHosts.IsKnown(uri))
            return;

        foreach (string tag in UrlTags)
        {
            if (activity.GetTagItem(tag) is string url)
            {
                activity.SetTag(
                    tag,
                    url.StartsWith('/') ? $"/{Redacted}" : $"{uri.Scheme}://{uri.Host}/{Redacted}"
                );
            }
        }
    }
}
