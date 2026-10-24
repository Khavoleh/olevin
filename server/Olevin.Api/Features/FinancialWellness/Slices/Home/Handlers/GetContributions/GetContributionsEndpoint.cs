using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions.Responses;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions;

/// <summary>
/// Gives what changed the index of the signed-in user since the previous snapshot.
/// </summary>
[Tags("Home")]
public static class GetContributionsEndpoint
{
    /// <summary>
    /// The snapshots the latest one and the one before it need for their windows.
    /// </summary>
    private const int LoadedMonths = IndexConstants.HistoryWindow + 1;

    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/home/contributions</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The contributions, or no data for the first snapshot (200), or a not found (404) without user settings or
    /// snapshots.
    /// </returns>
    [WolverineGet("/financial-wellness/v1/home/contributions", OperationId = "GetContributions")]
    [ProducesResponseType<Response<GetContributionsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetContributionsResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetContributionsResponse?>> Get(
        ClaimsPrincipal principal,
        FinancialWellnessDbContext db,
        IMessageBus bus,
        CancellationToken ct
    )
    {
        UserSettingsDto? userSettings = await bus.InvokeAsync<UserSettingsDto?>(
            new GetUserSettingsRequest(principal.GetSubject()),
            ct
        );

        if (userSettings is null)
        {
            return Response<GetContributionsResponse?>.Fail(
                HttpStatusCode.NotFound,
                "The user settings do not exist."
            );
        }

        Guid userId = userSettings.UserId;
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userId)
            .OrderByDescending(snapshot => snapshot.Year)
            .ThenByDescending(snapshot => snapshot.Month)
            .Take(LoadedMonths)
            .Select(snapshot => new Snapshot
            {
                Year = snapshot.Year,
                Month = snapshot.Month,
                FxDate = snapshot.FxDate,
                SelfRating = snapshot.SelfRating,
                FinalIndex = snapshot.FinalIndex,
                Amounts = snapshot.Amounts,
            })
            .ToListAsync(ct);

        if (snapshots.Count == 0)
        {
            return Response<GetContributionsResponse?>.Fail(
                HttpStatusCode.NotFound,
                "The snapshot does not exist."
            );
        }

        if (snapshots.Count == 1)
            return Response<GetContributionsResponse?>.Ok(null);

        List<EstimatedSnapshot> estimates =
            snapshots.Count < LoadedMonths
                ? await db
                    .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
                    .OrderByDescending(estimate => estimate.Year)
                    .ThenByDescending(estimate => estimate.Month)
                    .Take(LoadedMonths)
                    .Select(estimate => new EstimatedSnapshot
                    {
                        Year = estimate.Year,
                        Month = estimate.Month,
                        FxDate = estimate.FxDate,
                        ExpensesCurrency = estimate.ExpensesCurrency,
                        ExpensesAmount = estimate.ExpensesAmount,
                        SavedCurrency = estimate.SavedCurrency,
                        SavedAmount = estimate.SavedAmount,
                    })
                    .ToListAsync(ct)
                : [];

        SnapshotFigures[] history = await SnapshotHistory.LoadAsync(
            db,
            estimates,
            snapshots,
            userSettings.BaseCurrency,
            ct
        );
        MonthFigures[] months = [.. history.Select(item => item.Figures)];
        int previous = GetContributionsRules.PreviousSnapshot(months);

        return Response<GetContributionsResponse?>.Ok(
            GetContributionsRules.Contributions(
                months[previous],
                InputsCalculator.Calculate(months, previous),
                months[^1],
                InputsCalculator.Calculate(months, months.Length - 1),
                IndexPoints.Delta(snapshots[0].FinalIndex, snapshots[1].FinalIndex)
            )
        );
    }
}
