using System.Diagnostics;
using Olevin.Api.Infrastructure.ExchangeRate;

namespace Olevin.Tests.Infrastructure.ExchangeRate;

public sealed class ExchangeRateRedactionTests
{
    private const string Key = "a1b2c3d4e5f6a7b8c9d0e1f2";

    [Theory]
    [InlineData(
        "https://v6.exchangerate-api.com/v6/a1b2c3d4e5f6a7b8c9d0e1f2/latest/USD",
        "https://v6.exchangerate-api.com/v6/REDACTED/latest/USD"
    )]
    [InlineData("/v6/a1b2c3d4e5f6a7b8c9d0e1f2/latest/USD", "/v6/REDACTED/latest/USD")]
    [InlineData("https://example.com/health", "https://example.com/health")]
    public void Redact_HidesTheApiKey(string url, string expected) =>
        Assert.Equal(expected, ExchangeRateRedaction.Redact(url));

    [Fact]
    public void RedactSpan_HidesTheApiKeyInUrlTags()
    {
        using Activity activity = new("request");
        activity.SetTag("url.full", $"https://v6.exchangerate-api.com/v6/{Key}/latest/USD");
        activity.SetTag("http.url", $"https://v6.exchangerate-api.com/v6/{Key}/latest/USD");
        activity.SetTag("http.request.method", "GET");

        ExchangeRateRedaction.RedactSpan(activity);

        Assert.Equal(
            "https://v6.exchangerate-api.com/v6/REDACTED/latest/USD",
            activity.GetTagItem("url.full")
        );
        Assert.Equal(
            "https://v6.exchangerate-api.com/v6/REDACTED/latest/USD",
            activity.GetTagItem("http.url")
        );
        Assert.Equal("GET", activity.GetTagItem("http.request.method"));
    }
}
