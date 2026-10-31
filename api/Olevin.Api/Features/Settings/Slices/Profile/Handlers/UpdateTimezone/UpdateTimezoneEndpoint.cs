using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateTimezone;

/// <summary>
/// Changes the time zone of the signed-in user.
/// </summary>
[Tags("Profile")]
public static class UpdateTimezoneEndpoint
{
    /// <summary>
    /// Handles <c>PUT /settings/v1/user-settings/timezone</c>. The new zone only sets when the next snapshot days start.
    /// </summary>
    /// <param name="request">The new time zone.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The saved time zone (200), or a not found (404) when the user or their settings don't exist.
    /// </returns>
    [WolverinePut("/settings/v1/user-settings/timezone", OperationId = "UpdateTimezone")]
    [ProducesResponseType<Response<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<string>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<string>> Put(
        UpdateTimezoneRequest request,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        TimeProvider time,
        ILogger logger,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        UserSetting? setting = await db.UserSettings.FirstOrDefaultAsync(
            setting => setting.User.AuthSub == authSub,
            ct
        );

        if (setting is null)
            return Response<string>.Fail(HttpStatusCode.NotFound, "The settings do not exist.");

        if (setting.Timezone == request.Timezone)
            return Response<string>.Ok(setting.Timezone);

        setting.Timezone = request.Timezone;
        setting.UpdatedAt = time.GetUtcNow().UtcDateTime;

        logger.LogInformation("Updated the time zone of the user {UserId}.", setting.UserId);

        return Response<string>.Ok(setting.Timezone);
    }
}
