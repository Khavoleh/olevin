using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.PushSubscriptions;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.SubscribePush;

/// <summary>
/// Turns the push on the snapshot day on in a browser of the signed-in user.
/// </summary>
[Tags("Profile")]
public static class SubscribePushEndpoint
{
    /// <summary>
    /// Handles <c>POST /settings/v1/push-subscriptions</c>. A known endpoint gets the new keys and name, and moves to
    /// the signed-in user when another account subscribed it in the same browser before.
    /// </summary>
    /// <param name="request">The push subscription of the browser.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The identifier of the new subscription (201) or of the updated one (200), or a not found (404) when the user
    /// or their settings don't exist.
    /// </returns>
    [WolverinePost("/settings/v1/push-subscriptions", OperationId = "SubscribePush")]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<Guid>> Post(
        SubscribePushRequest request,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        TimeProvider time,
        ILogger logger,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        Guid? userId = await db
            .UserSettings.Where(setting => setting.User.AuthSub == authSub)
            .Select(setting => (Guid?)setting.UserId)
            .FirstOrDefaultAsync(ct);

        if (userId is not { } owner)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The settings do not exist.");

        DateTime now = time.GetUtcNow().UtcDateTime;
        PushSubscription? subscription = await db.PushSubscriptions.FirstOrDefaultAsync(
            stored => stored.Endpoint == request.Endpoint,
            ct
        );

        if (subscription is null)
        {
            subscription = new PushSubscription
            {
                Id = Guid.CreateVersion7(),
                UserId = owner,
                Endpoint = request.Endpoint,
                P256dh = request.P256dh,
                Auth = request.Auth,
                DeviceName = request.DeviceName.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
            };
            db.PushSubscriptions.Add(subscription);

            logger.LogInformation(
                "Added the push subscription {SubscriptionId} of the user {UserId}.",
                subscription.Id,
                owner
            );

            return Response<Guid>.Ok(subscription.Id, HttpStatusCode.Created);
        }

        if (subscription.UserId != owner)
        {
            subscription.UserId = owner;
            subscription.SnapshotPushYear = null;
            subscription.SnapshotPushMonth = null;
        }

        subscription.P256dh = request.P256dh;
        subscription.Auth = request.Auth;
        subscription.DeviceName = request.DeviceName.Trim();
        subscription.UpdatedAt = now;

        logger.LogInformation(
            "Updated the push subscription {SubscriptionId} of the user {UserId}.",
            subscription.Id,
            owner
        );

        return Response<Guid>.Ok(subscription.Id);
    }
}
