using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.SubscribePush;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.SubscribePush;

public sealed class SubscribePushValidatorTests
{
    private readonly SubscribePushValidator Validator = new();

    private static SubscribePushRequest Valid() =>
        new(
            "https://fcm.googleapis.com/fcm/send/dXk2Rm9aR3c6APA91bH",
            "BNcRdreALRFXTkOOUHK1EtK2wtaz5Ry4YfYCA_0QTpQtUbVlUls0VJXg7A8u-Ts1XbjhazAkj7I99e8QcYP7DkM",
            "tBHItJI5svbpez7KI4CCXg",
            "Chrome · macOS"
        );

    [Fact]
    public void Subscription_IsAccepted() =>
        Validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData("https://updates.push.services.mozilla.com/wpush/v2/gAAAAABk")]
    [InlineData("https://web.push.apple.com/QGuQyavXutnMH")]
    [InlineData("https://wns2-par02p.notify.windows.com/w/?token=BQYAAAB")]
    public void KnownPushService_IsAccepted(string endpoint) =>
        Validator
            .TestValidate(Valid() with { Endpoint = endpoint })
            .ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData("")]
    [InlineData("http://fcm.googleapis.com/fcm/send/abc")]
    [InlineData("https://example.com/push")]
    [InlineData("https://localhost:5001/health")]
    [InlineData("not a url")]
    public void OtherEndpoint_IsRejected(string endpoint) =>
        Validator
            .TestValidate(Valid() with { Endpoint = endpoint })
            .ShouldHaveValidationErrorFor(r => r.Endpoint);

    [Fact]
    public void TooLongEndpoint_IsRejected() =>
        Validator
            .TestValidate(
                Valid() with
                {
                    Endpoint = $"https://fcm.googleapis.com/fcm/send/{new string('a', 2048)}",
                }
            )
            .ShouldHaveValidationErrorFor(r => r.Endpoint);

    [Theory]
    [InlineData("")]
    [InlineData("abc+/def")]
    public void NotBase64UrlP256dh_IsRejected(string key) =>
        Validator
            .TestValidate(Valid() with { P256dh = key })
            .ShouldHaveValidationErrorFor(r => r.P256dh);

    [Theory]
    [InlineData("")]
    [InlineData("abc+/def")]
    public void NotBase64UrlAuth_IsRejected(string key) =>
        Validator
            .TestValidate(Valid() with { Auth = key })
            .ShouldHaveValidationErrorFor(r => r.Auth);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BlankDeviceName_IsRejected(string name) =>
        Validator
            .TestValidate(Valid() with { DeviceName = name })
            .ShouldHaveValidationErrorFor(r => r.DeviceName);

    [Fact]
    public void TooLongDeviceName_IsRejected() =>
        Validator
            .TestValidate(Valid() with { DeviceName = new string('a', 129) })
            .ShouldHaveValidationErrorFor(r => r.DeviceName);
}
