using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices;

/// <summary>
/// Gives the advice that raises the latest index of the signed-in user most.
/// </summary>
[Tags("Home")]
public static class GetAdvicesEndpoint
{
    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/home/advices</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The advice for the latest snapshot (200), or a not found (404) without user settings or snapshots.
    /// </returns>
    [WolverineGet("/financial-wellness/v1/home/advices", OperationId = "GetAdvices")]
    [ProducesResponseType<Response<GetAdvicesResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetAdvicesResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetAdvicesResponse>> Get(
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
            return Response<GetAdvicesResponse>.Fail(
                HttpStatusCode.NotFound,
                "The user settings do not exist."
            );
        }

        Guid userId = userSettings.UserId;
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userId)
            .OrderByDescending(snapshot => snapshot.Year)
            .ThenByDescending(snapshot => snapshot.Month)
            .Take(IndexConstants.HistoryWindow)
            .ToListAsync(ct);

        if (snapshots.Count == 0)
        {
            return Response<GetAdvicesResponse>.Fail(
                HttpStatusCode.NotFound,
                "The snapshot does not exist."
            );
        }

        List<EstimatedSnapshot> estimates =
            snapshots.Count < IndexConstants.HistoryWindow
                ? await db
                    .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
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
        CalculatedInputs inputs = InputsCalculator.Calculate(months, months.Length - 1);

        IReadOnlyList<AdviceItem> advice = AdviceBuilder.Build(months[^1], inputs);

        return Response<GetAdvicesResponse>.Ok(
            new GetAdvicesResponse(
                GetAdvicesMapping.StatusOf(advice, snapshots[0].FinalIndex),
                userSettings.BaseCurrency,
                GetAdvicesMapping.ToResponses(advice, inputs)
            )
        );
    }
}
