using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetIndex.Responses;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetIndex;

/// <summary>
/// Gives the latest index of the signed-in user.
/// </summary>
[Tags("Home")]
public static class GetIndexEndpoint
{
    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/home/index</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The index of the latest snapshot (200), or a not found (404) without user settings or snapshots.
    /// </returns>
    [WolverineGet("/financial-wellness/v1/home/index", OperationId = "GetIndex")]
    [ProducesResponseType<Response<GetIndexResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetIndexResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetIndexResponse>> Get(
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
            return Response<GetIndexResponse>.Fail(
                HttpStatusCode.NotFound,
                "The user settings do not exist."
            );
        }

        List<SnapshotDto> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userSettings.UserId)
            .OrderByDescending(snapshot => snapshot.Year)
            .ThenByDescending(snapshot => snapshot.Month)
            .Take(2)
            .Select(snapshot => new SnapshotDto(snapshot.Year, snapshot.Month, snapshot.FinalIndex))
            .ToListAsync(ct);

        if (snapshots.Count == 0)
        {
            return Response<GetIndexResponse>.Fail(
                HttpStatusCode.NotFound,
                "The snapshot does not exist."
            );
        }

        SnapshotDto latest = snapshots[0];
        SnapshotDto? previous = snapshots.ElementAtOrDefault(1);
        double value = IndexPoints.Round(latest.FinalIndex);

        return Response<GetIndexResponse>.Ok(
            new GetIndexResponse(
                latest.Year,
                latest.Month,
                value,
                Zones.Of(value),
                previous is null
                    ? null
                    : new IndexChangeResponse(
                        previous.Year,
                        previous.Month,
                        IndexPoints.Delta(latest.FinalIndex, previous.FinalIndex)
                    ),
                GetIndexRules.NextSnapshotDate(
                    new SnapshotMonth(latest.Year, latest.Month),
                    SnapshotMonth.Current(time.GetUtcNow(), userSettings.Timezone),
                    userSettings.SnapshotDay
                )
            )
        );
    }
}
