using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.GetSettings;

/// <summary>
/// Gives every setting of the signed-in user for the profile page.
/// </summary>
[Tags("Profile")]
public static class GetSettingsEndpoint
{
    /// <summary>
    /// Handles <c>GET /settings/v1/user-settings</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The settings of the user (200), or a not found (404) when the user or their settings don't exist.
    /// </returns>
    [WolverineGet("/settings/v1/user-settings", OperationId = "GetSettings")]
    [ProducesResponseType<Response<GetSettingsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetSettingsResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetSettingsResponse>> Get(
        ClaimsPrincipal principal,
        SettingsDbContext db,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();
        string? username = principal.GetUsername();
        string? email = principal.GetEmail();

        GetSettingsResponse? settings = await db
            .UserSettings.Where(setting => setting.User.AuthSub == authSub)
            .Select(setting => new GetSettingsResponse(
                username,
                email,
                setting.User.CreatedAt,
                setting.BaseCurrency,
                setting.Locale,
                setting.SnapshotDay,
                setting.Timezone,
                setting.ResearchConsent,
                true
            ))
            .FirstOrDefaultAsync(ct);

        return settings is null
            ? Response<GetSettingsResponse>.Fail(
                HttpStatusCode.NotFound,
                "The settings do not exist."
            )
            : Response<GetSettingsResponse>.Ok(settings);
    }
}
