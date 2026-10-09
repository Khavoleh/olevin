using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.Users;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Handlers.ConfigureUserSettings;

/// <summary>
/// Creates the settings of the signed-in user during onboarding.
/// </summary>
[Tags("Settings")]
public static class ConfigureUserSettingsEndpoint
{
    /// <summary>
    /// Handles <c>POST /settings/v1/user-settings</c>.
    /// </summary>
    /// <param name="request">The settings of the user.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The created settings (201), a not found (404) when the user does not exist,
    /// or a conflict (409) when the settings already exist.
    /// </returns>
    [WolverinePost("/settings/v1/user-settings")]
    [ProducesResponseType<Response<bool>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Response<bool>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Response<bool>>(StatusCodes.Status409Conflict)]
    public static async Task<Response<bool>> Post(
        ConfigureUserSettingsRequest request,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        User? user = await db
            .Users.Include(user => user.Setting)
            .FirstOrDefaultAsync(user => user.AuthSub == authSub, ct);

        if (user is null)
            return Response<bool>.Fail(HttpStatusCode.NotFound, "The user does not exist.");

        if (user.Setting is not null)
            return Response<bool>.Fail(HttpStatusCode.Conflict, "The settings already exist.");

        user.Setting = new UserSetting
        {
            BaseCurrency = request.BaseCurrency,
            Locale = request.Locale,
            SnapshotDay = request.SnapshotDay,
            Timezone = request.Timezone,
        };

        return Response<bool>.Ok(true, HttpStatusCode.Created);
    }
}
