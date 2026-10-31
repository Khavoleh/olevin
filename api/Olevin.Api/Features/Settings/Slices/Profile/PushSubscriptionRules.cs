using FluentValidation;
using Olevin.Api.Infrastructure.WebPush;

namespace Olevin.Api.Features.Settings.Slices.Profile;

/// <summary>
/// The rules of a push subscription, shared by the requests that name one.
/// </summary>
public static class PushSubscriptionRules
{
    private const int MaxEndpointLength = 2048;

    /// <summary>
    /// Requires an HTTPS URL of a known push service that fits the column.
    /// </summary>
    /// <typeparam name="T">The validated request.</typeparam>
    /// <param name="rule">The rule of the endpoint.</param>
    /// <returns>The rule.</returns>
    public static IRuleBuilderOptions<T, string> Endpoint<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty()
            .WithMessage("Endpoint is required.")
            .MaximumLength(MaxEndpointLength)
            .WithMessage($"Endpoint cannot be longer than {MaxEndpointLength} characters.")
            .Must(PushServiceHosts.IsKnown)
            .WithMessage("Endpoint must be an HTTPS URL of a known push service.");
}
