using System.Diagnostics;
using Olevin.Api.Infrastructure.WebPush;

namespace Olevin.Tests.Infrastructure.WebPush;

public sealed class WebPushRedactionTests
{
    private const string Endpoint = "https://fcm.googleapis.com/fcm/send/dXk2Rm9aR3c6APA91bH";

    [Fact]
    public void RedactSpan_HidesTheEndpointInUrlTags()
    {
        using Activity activity = new("request");
        using HttpRequestMessage request = new(HttpMethod.Post, Endpoint);
        activity.SetTag("url.full", Endpoint);
        activity.SetTag("url.path", "/fcm/send/dXk2Rm9aR3c6APA91bH");
        activity.SetTag("http.request.method", "POST");

        WebPushRedaction.RedactSpan(activity, request);

        Assert.Equal("https://fcm.googleapis.com/REDACTED", activity.GetTagItem("url.full"));
        Assert.Equal("/REDACTED", activity.GetTagItem("url.path"));
        Assert.Equal("POST", activity.GetTagItem("http.request.method"));
    }

    [Fact]
    public void RedactSpan_KeepsOtherRequests()
    {
        const string url = "https://v6.exchangerate-api.com/v6/REDACTED/latest/USD";
        using Activity activity = new("request");
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        activity.SetTag("url.full", url);

        WebPushRedaction.RedactSpan(activity, request);

        Assert.Equal(url, activity.GetTagItem("url.full"));
    }
}
