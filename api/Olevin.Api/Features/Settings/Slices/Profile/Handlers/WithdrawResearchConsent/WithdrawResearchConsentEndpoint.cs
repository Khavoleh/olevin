using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.Users;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.WithdrawResearchConsent;

/// <summary>
/// Withdraws the research consent of the signed-in user and deletes their CFPB response.
/// </summary>
[Tags("Profile")]
public static class WithdrawResearchConsentEndpoint
{
    /// <summary>
    /// Handles <c>DELETE /settings/v1/user-settings/research-consent</c>. The consent cannot be given again.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// No content (204), or a not found (404) when the user or their settings don't exist.
    /// </returns>
    [WolverineDelete(
        "/settings/v1/user-settings/research-consent",
        OperationId = "WithdrawResearchConsent"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<Guid>> Delete(
        ClaimsPrincipal principal,
        SettingsDbContext db,
        TimeProvider time,
        ILogger logger,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        User? user = await db
            .Users.Include(user => user.Setting)
            .Include(user => user.CfpbResponse)
            .FirstOrDefaultAsync(user => user.AuthSub == authSub, ct);

        if (user?.Setting is null)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The settings do not exist.");

        if (!user.Setting.ResearchConsent && user.CfpbResponse is null)
            return Response<Guid>.NoContent();

        user.Setting.ResearchConsent = false;
        user.Setting.UpdatedAt = time.GetUtcNow().UtcDateTime;

        if (user.CfpbResponse is not null)
            db.CfpbResponses.Remove(user.CfpbResponse);

        logger.LogInformation("Withdrew the research consent of the user {UserId}.", user.Id);

        return Response<Guid>.NoContent();
    }
}
