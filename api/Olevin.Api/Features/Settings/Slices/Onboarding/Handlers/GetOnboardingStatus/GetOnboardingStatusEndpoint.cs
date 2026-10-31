using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.HasFirstSnapshot;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.Users;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.GetOnboardingStatus;

/// <summary>
/// Tells how far the signed-in user has gone through the onboarding.
/// </summary>
[Tags("Onboarding")]
public static class GetOnboardingStatusEndpoint
{
    /// <summary>
    /// Handles <c>GET /settings/v1/user/onboarding</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The part of the onboarding the user has to go through next (200).</returns>
    [WolverineGet("/settings/v1/user/onboarding", OperationId = "GetOnboardingStatus")]
    [ProducesResponseType<Response<OnboardingStatus>>(StatusCodes.Status200OK)]
    [NonTransactional]
    public static async Task<Response<OnboardingStatus>> Get(
        ClaimsPrincipal principal,
        SettingsDbContext db,
        IMessageBus bus,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        User? user = await db
            .Users.Include(user => user.Setting)
            .Include(user => user.CfpbResponse)
            .FirstOrDefaultAsync(user => user.AuthSub == authSub, ct);

        bool hasFirstSnapshot =
            user is { Setting: not null }
            && await bus.InvokeAsync<bool>(new HasFirstSnapshotRequest(user.Id), ct);

        OnboardingStatus status = user switch
        {
            null => OnboardingStatus.NotExist,
            { Setting: null } => OnboardingStatus.Settings,
            _ when !hasFirstSnapshot => OnboardingStatus.Onboarding,
            { Setting.ResearchConsent: true, CfpbResponse: null } => OnboardingStatus.CfpbResponse,
            _ => OnboardingStatus.Completed,
        };

        return Response<OnboardingStatus>.Ok(status);
    }
}
