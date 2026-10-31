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

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.ConfigureUserSettings;

/// <summary>
/// Creates the settings of the signed-in user during onboarding.
/// </summary>
[Tags("Onboarding")]
public static class ConfigureUserSettingsEndpoint
{
    /// <summary>
    /// Handles <c>POST /settings/v1/user-settings</c>.
    /// </summary>
    /// <param name="request">The settings of the user.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The identifier of the user the settings were created for (201),
    /// a not found (404) when the user does not exist,
    /// or a conflict (409) when the settings already exist.
    /// </returns>
    [WolverinePost("/settings/v1/user-settings", OperationId = "ConfigureUserSettings")]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status409Conflict)]
    public static async Task<Response<Guid>> Post(
        ConfigureUserSettingsRequest request,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        ILogger logger,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        User? user = await db
            .Users.Include(user => user.Setting)
            .FirstOrDefaultAsync(user => user.AuthSub == authSub, ct);

        if (user is null)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The user does not exist.");

        if (user.Setting is not null)
            return Response<Guid>.Fail(HttpStatusCode.Conflict, "The settings already exist.");

        user.Setting = new UserSetting
        {
            BaseCurrency = request.BaseCurrency,
            Locale = request.Locale,
            SnapshotDay = request.SnapshotDay,
            Timezone = request.Timezone,
            ResearchConsent = request.ResearchConsent,
        };

        logger.LogInformation("Created the settings of the user {UserId}.", user.Id);

        return Response<Guid>.Ok(user.Id, HttpStatusCode.Created);
    }
}
