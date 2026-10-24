using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetOpenMonths.Responses;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetOpenMonths;

/// <summary>
/// Gives the months the signed-in user may create a snapshot for.
/// </summary>
[Tags("Snapshots")]
public static class GetOpenMonthsEndpoint
{
    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/snapshots/open-months</c>. A missed month stays open.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The open months (200), or a not found (404) without user settings or without the first snapshot.
    /// </returns>
    [WolverineGet("/financial-wellness/v1/snapshots/open-months", OperationId = "GetOpenMonths")]
    [ProducesResponseType<Response<GetOpenMonthsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetOpenMonthsResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetOpenMonthsResponse>> Get(
        ClaimsPrincipal principal,
        FinancialWellnessDbContext db,
        IMessageBus bus,
        TimeProvider time,
        CancellationToken ct
    )
    {
        UserSettingsDto? userSettings = await bus.InvokeAsync<UserSettingsDto?>(
            new GetUserSettingsRequest(principal.GetSubject()),
            ct
        );

        if (userSettings is null)
        {
            return Response<GetOpenMonthsResponse>.Fail(
                HttpStatusCode.NotFound,
                "The user settings do not exist."
            );
        }

        Guid userId = userSettings.UserId;
        SnapshotMonth[] estimates =
        [
            .. await db
                .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
                .Select(estimate => new SnapshotMonth(estimate.Year, estimate.Month))
                .ToListAsync(ct),
        ];

        if (estimates.Length == 0)
        {
            return Response<GetOpenMonthsResponse>.Fail(
                HttpStatusCode.NotFound,
                "The first snapshot does not exist."
            );
        }

        SnapshotMonth[] taken =
        [
            .. await db
                .Snapshots.Where(snapshot => snapshot.UserId == userId)
                .Select(snapshot => new SnapshotMonth(snapshot.Year, snapshot.Month))
                .ToListAsync(ct),
        ];
        SnapshotMonth earliest = OpenMonths.Earliest(estimates);
        SnapshotMonth latest = OpenMonths.Latest(
            SnapshotMonth.Today(time.GetUtcNow(), userSettings.Timezone),
            userSettings.SnapshotDay
        );

        return Response<GetOpenMonthsResponse>.Ok(
            new GetOpenMonthsResponse(
                [
                    .. OpenMonths
                        .Free(earliest, latest, taken)
                        .Select(month => new OpenMonthResponse(month.Year, month.Month)),
                ],
                OpenMonths.OpensOn(earliest, latest, taken, userSettings.SnapshotDay)
            )
        );
    }
}
