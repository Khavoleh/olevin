using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.PushSubscriptions;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.DeletePushSubscription;

/// <summary>
/// Turns the push off in a browser of the signed-in user.
/// </summary>
[Tags("Profile")]
public static class DeletePushSubscriptionEndpoint
{
    /// <summary>
    /// Handles <c>DELETE /settings/v1/push-subscriptions/{id}</c>. Any browser of the user can be removed, not only
    /// the one that sends the request.
    /// </summary>
    /// <param name="id">The subscription identifier.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>No content (204), or a not found (404) when the user has no such subscription.</returns>
    [WolverineDelete(
        "/settings/v1/push-subscriptions/{id}",
        OperationId = "DeletePushSubscription"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<Guid>> Delete(
        Guid id,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        ILogger logger,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        PushSubscription? subscription = await db.PushSubscriptions.FirstOrDefaultAsync(
            stored => stored.Id == id && stored.User.AuthSub == authSub,
            ct
        );

        if (subscription is null)
        {
            return Response<Guid>.Fail(
                HttpStatusCode.NotFound,
                "The push subscription does not exist."
            );
        }

        db.PushSubscriptions.Remove(subscription);

        logger.LogInformation(
            "Deleted the push subscription {SubscriptionId} of the user {UserId}.",
            id,
            subscription.UserId
        );

        return Response<Guid>.NoContent();
    }
}
