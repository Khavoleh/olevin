using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.PushSubscriptions;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.SyncPushSubscription;

/// <summary>
/// Confirms that a browser of the signed-in user still gets the push.
/// </summary>
[Tags("Profile")]
public static class SyncPushSubscriptionEndpoint
{
    /// <summary>
    /// Handles <c>PUT /settings/v1/push-subscriptions</c>. Never adds a subscription, so a browser removed from another
    /// device does not come back on its own.
    /// </summary>
    /// <param name="request">The push subscription of the browser.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The identifier of the subscription (200), or a not found (404) when the user has no such subscription.
    /// </returns>
    [WolverinePut("/settings/v1/push-subscriptions", OperationId = "SyncPushSubscription")]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<Guid>> Put(
        SyncPushSubscriptionRequest request,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        TimeProvider time,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        PushSubscription? subscription = await db.PushSubscriptions.FirstOrDefaultAsync(
            stored => stored.Endpoint == request.Endpoint && stored.User.AuthSub == authSub,
            ct
        );

        if (subscription is null)
        {
            return Response<Guid>.Fail(
                HttpStatusCode.NotFound,
                "The push subscription does not exist."
            );
        }

        subscription.UpdatedAt = time.GetUtcNow().UtcDateTime;

        return Response<Guid>.Ok(subscription.Id);
    }
}
