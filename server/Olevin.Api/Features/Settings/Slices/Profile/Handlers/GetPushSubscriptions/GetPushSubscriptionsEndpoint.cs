using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.GetPushSubscriptions;

/// <summary>
/// Gives the browsers of the signed-in user that get the push on the snapshot day.
/// </summary>
[Tags("Profile")]
public static class GetPushSubscriptionsEndpoint
{
    /// <summary>
    /// Handles <c>GET /settings/v1/push-subscriptions</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The subscriptions of the user, the newest first (200); empty when the push is off everywhere.</returns>
    [WolverineGet("/settings/v1/push-subscriptions", OperationId = "GetPushSubscriptions")]
    [ProducesResponseType<Response<IReadOnlyList<PushSubscriptionResponse>>>(
        StatusCodes.Status200OK
    )]
    [NonTransactional]
    public static async Task<Response<IReadOnlyList<PushSubscriptionResponse>>> Get(
        ClaimsPrincipal principal,
        SettingsDbContext db,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        List<PushSubscriptionResponse> subscriptions = await db
            .PushSubscriptions.Where(subscription => subscription.User.AuthSub == authSub)
            .OrderByDescending(subscription => subscription.CreatedAt)
            .Select(subscription => new PushSubscriptionResponse(
                subscription.Id,
                subscription.Endpoint,
                subscription.DeviceName,
                subscription.CreatedAt,
                subscription.UpdatedAt
            ))
            .ToListAsync(ct);

        return Response<IReadOnlyList<PushSubscriptionResponse>>.Ok(subscriptions);
    }
}
