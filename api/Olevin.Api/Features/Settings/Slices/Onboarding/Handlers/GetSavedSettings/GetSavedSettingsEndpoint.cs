using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.GetSavedSettings;

/// <summary>
/// Gives the settings the signed-in user saved during onboarding.
/// </summary>
[Tags("Onboarding")]
public static class GetSavedSettingsEndpoint
{
    /// <summary>
    /// Handles <c>GET /settings/v1/user/onboarding/settings</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The base currency, snapshot day and time zone of the user (200), or a not found (404) when the user or their
    /// settings don't exist.
    /// </returns>
    [WolverineGet("/settings/v1/user/onboarding/settings", OperationId = "GetSavedSettings")]
    [ProducesResponseType<Response<GetSavedSettingsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetSavedSettingsResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetSavedSettingsResponse>> Get(
        ClaimsPrincipal principal,
        SettingsDbContext db,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        GetSavedSettingsResponse? settings = await db
            .UserSettings.Where(setting => setting.User.AuthSub == authSub)
            .Select(setting => new GetSavedSettingsResponse(
                setting.BaseCurrency,
                setting.SnapshotDay,
                setting.Timezone
            ))
            .FirstOrDefaultAsync(ct);

        return settings is null
            ? Response<GetSavedSettingsResponse>.Fail(
                HttpStatusCode.NotFound,
                "The settings do not exist."
            )
            : Response<GetSavedSettingsResponse>.Ok(settings);
    }
}
