using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.CreateFirstSnapshot;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.CreateFirstSnapshot;

/// <summary>
/// Saves the first snapshot of the signed-in user during onboarding, together with the estimates of the 2 months
/// before it.
/// </summary>
[Tags("Onboarding")]
public static class CreateFirstSnapshotEndpoint
{
    /// <summary>
    /// Handles <c>POST /settings/v1/user/onboarding/first-snapshot</c>. The snapshot is for the current month in
    /// the user's time zone.
    /// </summary>
    /// <param name="snapshot">The first snapshot and the estimates.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="bus">The message bus, to save the snapshot in the financial wellness feature.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The identifier of the created snapshot (201), a not found (404) when the user or their settings don't exist,
    /// or a conflict (409) when the user already has snapshots.
    /// </returns>
    [WolverinePost(
        "/settings/v1/user/onboarding/first-snapshot",
        OperationId = "CreateFirstSnapshot"
    )]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status409Conflict)]
    public static Task<Response<Guid>> Post(
        FirstSnapshot snapshot,
        ClaimsPrincipal principal,
        IMessageBus bus,
        CancellationToken ct
    ) =>
        bus.InvokeAsync<Response<Guid>>(
            new CreateFirstSnapshotRequest(principal.GetSubject(), snapshot),
            ct
        );
}
