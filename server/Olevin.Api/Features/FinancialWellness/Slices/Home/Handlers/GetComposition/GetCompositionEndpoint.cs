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
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Responses;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Helpers;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition;

/// <summary>
/// Gives what the latest index of the signed-in user is made of.
/// </summary>
[Tags("Home")]
public static class GetCompositionEndpoint
{
    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/home/composition</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The composition of the latest snapshot (200), or a not found (404) without user settings or snapshots.
    /// </returns>
    [WolverineGet("/financial-wellness/v1/home/composition", OperationId = "GetComposition")]
    [ProducesResponseType<Response<GetCompositionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetCompositionResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetCompositionResponse>> Get(
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
            return Response<GetCompositionResponse>.Fail(
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
            .Select(snapshot => new Snapshot
            {
                Year = snapshot.Year,
                Month = snapshot.Month,
                FxDate = snapshot.FxDate,
                SelfRating = snapshot.SelfRating,
                Amounts = snapshot.Amounts,
                CashFlow = snapshot.CashFlow,
                Buffer = snapshot.Buffer,
            })
            .ToListAsync(ct);

        if (snapshots.Count == 0)
        {
            return Response<GetCompositionResponse>.Fail(
                HttpStatusCode.NotFound,
                "The snapshot does not exist."
            );
        }

        List<EstimatedSnapshot> estimates =
            snapshots.Count < IndexConstants.HistoryWindow
                ? await db
                    .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
                    .OrderByDescending(estimate => estimate.Year)
                    .ThenByDescending(estimate => estimate.Month)
                    .Take(IndexConstants.HistoryWindow)
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
        CalculatedInputs inputs = InputsCalculator.Calculate(
            [.. history.Select(item => item.Figures)],
            history.Length - 1
        );
        IndicatorVector raw = inputs.Raw;
        MonthFigures month = history[^1].Figures;
        Snapshot latest = snapshots[0];

        return Response<GetCompositionResponse>.Ok(
            new GetCompositionResponse(
                new CashFlowResponse(
                    IndexPoints.Round(latest.CashFlow),
                    GetCompositionRules.SubsystemTermOf(latest.CashFlow),
                    ShownIndicators.OfIncome(raw.SavingsRate, month),
                    ShownIndicators.OfIncome(raw.DebtToIncome, month)
                ),
                new BufferResponse(
                    IndexPoints.Round(latest.Buffer),
                    GetCompositionRules.SubsystemTermOf(latest.Buffer),
                    ShownIndicators.ReserveMonths(inputs, month),
                    ShownIndicators.ExpenseVariation(inputs)
                ),
                new SavingsRegularityResponse(
                    GetCompositionRules.SavingsRegularityTermOf(raw.SavingsRegularity),
                    GetCompositionRules.SavingsMonths(history)
                )
            )
        );
    }
}
