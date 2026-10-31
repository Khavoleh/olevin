using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Users.Handlers;

/// <summary>
/// Deletes the data of a user after Logto reports that it deleted their account.
/// </summary>
[Tags("Users")]
public static class UserDeletedEndpoint
{
    private const int MaxBodyBytes = 64 * 1024;

    /// <summary>
    /// Handles <c>POST /settings/v1/webhooks/user</c>, the Logto webhook that is signed with the webhook signing key.
    /// The user and everything that references them are deleted.
    /// </summary>
    /// <param name="request">The request, whose raw body is signed.</param>
    /// <param name="options">The Logto settings.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// No content (204), a bad request (400) when the body is not a valid payload, or an unauthorized (401) when the
    /// signature does not match.
    /// </returns>
    [AllowAnonymous]
    [ExcludeFromDescription]
    [WolverinePost("/settings/v1/webhooks/user", OperationId = "UserDeleted")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status401Unauthorized)]
    public static async Task<Response<Guid>> Post(
        HttpRequest request,
        IOptions<LogtoOptions> options,
        SettingsDbContext db,
        ILogger logger,
        CancellationToken ct
    )
    {
        IHttpMaxRequestBodySizeFeature? limit =
            request.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();

        if (limit is { IsReadOnly: false })
            limit.MaxRequestBodySize = MaxBodyBytes;

        using MemoryStream body = new();
        await request.Body.CopyToAsync(body, ct);

        if (
            !LogtoWebhookSignature.IsValid(
                options.Value.WebhookSigningKey,
                body.ToArray(),
                request.Headers[LogtoWebhookSignature.HeaderName]
            )
        )
        {
            logger.LogWarning("A Logto webhook request has an invalid signature.");

            return Response<Guid>.Fail(HttpStatusCode.Unauthorized, "The signature is not valid.");
        }

        UserDeletedPayload? payload = UserDeletedPayload.Parse(body.ToArray());

        if (payload is null)
            return Response<Guid>.Fail(HttpStatusCode.BadRequest, "The payload is not valid.");

        if (payload.DeletedUserId is not { } authSub)
            return Response<Guid>.NoContent();

        await db.Users.Where(user => user.AuthSub == authSub).ExecuteDeleteAsync(ct);

        logger.LogInformation("Deleted the data of user after Logto deleted the account.");

        return Response<Guid>.NoContent();
    }
}
