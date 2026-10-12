using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Olevin.Api.Infrastructure.ExchangeRate;

/// <summary>
/// Hides the ExchangeRate-API key, which the API requires in the URL path, from traces.
/// </summary>
public static partial class ExchangeRateRedaction
{
    private const string Redacted = "REDACTED";

    /// <summary>
    /// The span tags that may hold the request URL, in the legacy and the stable HTTP semantic conventions.
    /// </summary>
    private static readonly string[] UrlTags = ["url.full", "url.path", "http.url", "http.target"];

    /// <summary>
    /// Replaces the API key in a URL or a path.
    /// </summary>
    /// <param name="url">The URL or the path of a request.</param>
    /// <returns>The same URL with <c>REDACTED</c> in place of the API key.</returns>
    public static string Redact(string url) => ApiKeySegment().Replace(url, $"$1{Redacted}");

    /// <summary>
    /// Removes the API key from the URL tags of an HTTP client span. Meant for the
    /// <c>EnrichWithHttpRequestMessage</c> option of the HTTP client instrumentation.
    /// </summary>
    /// <param name="activity">The span of the request.</param>
    public static void RedactSpan(Activity activity)
    {
        foreach (string tag in UrlTags)
        {
            if (activity.GetTagItem(tag) is string url)
                activity.SetTag(tag, Redact(url));
        }
    }

    [GeneratedRegex("(/v6/)[^/?#]+")]
    private static partial Regex ApiKeySegment();
}
