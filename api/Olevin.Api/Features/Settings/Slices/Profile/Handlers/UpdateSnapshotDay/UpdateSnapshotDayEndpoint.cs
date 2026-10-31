using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateSnapshotDay;

/// <summary>
/// Changes the snapshot day of the signed-in user.
/// </summary>
[Tags("Profile")]
public static class UpdateSnapshotDayEndpoint
{
    /// <summary>
    /// Handles <c>PUT /settings/v1/user-settings/snapshot-day</c>. The new day applies to the next snapshots only.
    /// </summary>
    /// <param name="request">The new snapshot day.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The saved snapshot day (200), or a not found (404) when the user or their settings don't exist.
    /// </returns>
    [WolverinePut("/settings/v1/user-settings/snapshot-day", OperationId = "UpdateSnapshotDay")]
    [ProducesResponseType<Response<short>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<short>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<short>> Put(
        UpdateSnapshotDayRequest request,
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
            return Response<short>.Fail(HttpStatusCode.NotFound, "The settings do not exist.");

        if (setting.SnapshotDay == request.SnapshotDay)
            return Response<short>.Ok(setting.SnapshotDay);

        setting.SnapshotDay = request.SnapshotDay;
        setting.UpdatedAt = time.GetUtcNow().UtcDateTime;

        logger.LogInformation("Updated the snapshot day of the user {UserId}.", setting.UserId);

        return Response<short>.Ok(setting.SnapshotDay);
    }
}
