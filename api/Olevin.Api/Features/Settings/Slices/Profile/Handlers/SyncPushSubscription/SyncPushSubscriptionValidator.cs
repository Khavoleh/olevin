using FluentValidation;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.SyncPushSubscription;

/// <summary>
/// Validates the push subscription a browser still has.
/// </summary>
public sealed class SyncPushSubscriptionValidator : AbstractValidator<SyncPushSubscriptionRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SyncPushSubscriptionValidator"/> class.
    /// </summary>
    public SyncPushSubscriptionValidator()
    {
        RuleFor(request => request.Endpoint).Endpoint();
    }
}
