using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Trend.Handlers;

/// <summary>
/// Gives the index and its subsystems of the signed-in user month by month.
/// </summary>
[Tags("Trend")]
public static class GetTrendEndpoint
{
    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/trend/{period}</c>.
    /// </summary>
    /// <param name="period">The months the trend covers.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// Every month of the period, the oldest first, with <see langword="null"/> values for months without a snapshot
    /// and empty when the period has no snapshot (200), or a not found (404) without user settings or snapshots.
    /// </returns>
    [WolverineGet("/financial-wellness/v1/trend/{period}", OperationId = "GetTrend")]
    [ProducesResponseType<Response<IReadOnlyList<TrendPointResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<IReadOnlyList<TrendPointResponse>>>(
        StatusCodes.Status404NotFound
    )]
    [NonTransactional]
    public static async Task<Response<IReadOnlyList<TrendPointResponse>>> Get(
        TrendPeriod period,
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
            return Response<IReadOnlyList<TrendPointResponse>>.Fail(
                HttpStatusCode.NotFound,
                "The user settings do not exist."
            );
        }

        List<SnapshotDto> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userSettings.UserId)
            .Select(snapshot => new SnapshotDto(
                snapshot.Year,
                snapshot.Month,
                snapshot.FinalIndex,
                snapshot.CashFlow,
                snapshot.Buffer
            ))
            .ToListAsync(ct);

        return snapshots.Count == 0
            ? Response<IReadOnlyList<TrendPointResponse>>.Fail(
                HttpStatusCode.NotFound,
                "The snapshot does not exist."
            )
            : Response<IReadOnlyList<TrendPointResponse>>.Ok(
                GetTrendRules.Points(
                    period,
                    OpenMonths.Latest(
                        SnapshotMonth.Today(time.GetUtcNow(), userSettings.Timezone),
                        userSettings.SnapshotDay
                    ),
                    snapshots
                )
            );
    }
}
