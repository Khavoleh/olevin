using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.History.Handlers.GetSnapshots.Responses;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.History.Handlers.GetSnapshots;

/// <summary>
/// Gives the snapshot history of the signed-in user.
/// </summary>
[Tags("History")]
public static class GetSnapshotsEndpoint
{
    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/snapshots</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// Every snapshot with the estimates (200), or a not found (404) without user settings or snapshots.
    /// </returns>
    [WolverineGet("/financial-wellness/v1/snapshots", OperationId = "GetSnapshots")]
    [ProducesResponseType<Response<GetSnapshotsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetSnapshotsResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetSnapshotsResponse>> Get(
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
            return Response<GetSnapshotsResponse>.Fail(
                HttpStatusCode.NotFound,
                "The user settings do not exist."
            );
        }

        Guid userId = userSettings.UserId;
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userId)
            .OrderByDescending(snapshot => snapshot.Year)
            .ThenByDescending(snapshot => snapshot.Month)
            .Select(snapshot => new Snapshot
            {
                Id = snapshot.Id,
                Year = snapshot.Year,
                Month = snapshot.Month,
                FxDate = snapshot.FxDate,
                SelfRating = snapshot.SelfRating,
                Amounts = snapshot.Amounts,
                FinalIndex = snapshot.FinalIndex,
            })
            .ToListAsync(ct);

        if (snapshots.Count == 0)
        {
            return Response<GetSnapshotsResponse>.Fail(
                HttpStatusCode.NotFound,
                "The snapshot does not exist."
            );
        }

        List<EstimatedSnapshot> estimates = await db
            .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
            .OrderByDescending(estimate => estimate.Year)
            .ThenByDescending(estimate => estimate.Month)
            .ToListAsync(ct);
        SnapshotFigures[] history = await SnapshotHistory.LoadAsync(
            db,
            estimates,
            snapshots,
            userSettings.BaseCurrency,
            ct
        );
        Dictionary<SnapshotMonth, MonthFigures> figures = history.ToDictionary(
            item => item.Month,
            item => item.Figures
        );
        double[] indexes = [.. snapshots.Select(snapshot => snapshot.FinalIndex)];
        short snapshotDay = userSettings.SnapshotDay;

        return Response<GetSnapshotsResponse>.Ok(
            new GetSnapshotsResponse(
                userSettings.BaseCurrency,
                [
                    .. snapshots.Select(
                        (snapshot, position) =>
                        {
                            MonthFigures month = figures[SnapshotHistoryMapping.MonthOf(snapshot)];
                            double index = IndexPoints.Round(snapshot.FinalIndex);

                            return new SnapshotResponse(
                                snapshot.Id,
                                snapshot.Year,
                                snapshot.Month,
                                index,
                                Zones.Of(index),
                                GetSnapshotsRules.ChangeAt(indexes, position),
                                GetSnapshotsRules.RoundMoney(month.Expenses),
                                GetSnapshotsRules.RoundMoney(month.Saved)
                            );
                        }
                    ),
                ],
                [
                    .. estimates.Select(estimate =>
                    {
                        MonthFigures month = figures[SnapshotHistoryMapping.MonthOf(estimate)];

                        return new EstimateResponse(
                            estimate.Id,
                            estimate.Year,
                            estimate.Month,
                            GetSnapshotsRules.RoundMoney(month.Expenses),
                            GetSnapshotsRules.RoundMoney(month.Saved)
                        );
                    }),
                ],
                OpenMonths.OpensOn(
                    OpenMonths.Earliest(estimates.Select(SnapshotHistoryMapping.MonthOf)),
                    OpenMonths.Latest(
                        SnapshotMonth.Today(time.GetUtcNow(), userSettings.Timezone),
                        snapshotDay
                    ),
                    [.. snapshots.Select(SnapshotHistoryMapping.MonthOf)],
                    snapshotDay
                )
            )
        );
    }
}
