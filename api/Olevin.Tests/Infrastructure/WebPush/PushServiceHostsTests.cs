using Olevin.Api.Infrastructure.WebPush;

namespace Olevin.Tests.Infrastructure.WebPush;

public sealed class PushServiceHostsTests
{
    [Theory]
    [InlineData("https://fcm.googleapis.com/fcm/send/abc")]
    [InlineData("https://FCM.googleapis.com/fcm/send/abc")]
    [InlineData("https://updates.push.services.mozilla.com/wpush/v2/abc")]
    [InlineData("https://web.push.apple.com/abc")]
    [InlineData("https://wns2-par02p.notify.windows.com/w/?token=abc")]
    public void PushService_IsKnown(string endpoint) =>
        Assert.True(PushServiceHosts.IsKnown(endpoint));

    [Theory]
    [InlineData("http://fcm.googleapis.com/fcm/send/abc")]
    [InlineData("https://fcm.googleapis.com.example.com/abc")]
    [InlineData("https://examplenotify.windows.com/w/?token=abc")]
    [InlineData("https://localhost/abc")]
    [InlineData("/fcm/send/abc")]
    [InlineData("not a url")]
    public void OtherUrl_IsNotKnown(string endpoint) =>
        Assert.False(PushServiceHosts.IsKnown(endpoint));
}
