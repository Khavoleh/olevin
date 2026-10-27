using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.SyncPushSubscription;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.SyncPushSubscription;

public sealed class SyncPushSubscriptionValidatorTests
{
    private readonly SyncPushSubscriptionValidator Validator = new();

    [Fact]
    public void KnownPushService_IsAccepted() =>
        Validator
            .TestValidate(new SyncPushSubscriptionRequest("https://web.push.apple.com/QGuQyavX"))
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void OtherEndpoint_IsRejected() =>
        Validator
            .TestValidate(new SyncPushSubscriptionRequest("https://example.com/push"))
            .ShouldHaveValidationErrorFor(r => r.Endpoint);
}
