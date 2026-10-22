using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.UpdateEstimatedSnapshot;

/// <summary>
/// Changes an estimate of the signed-in user.
/// </summary>
[Tags("Snapshots")]
public static class UpdateEstimatedSnapshotEndpoint
{
    /// <summary>
    /// Handles <c>PUT /financial-wellness/v1/snapshots/estimates/{id}</c>. Every snapshot is later than the estimates,
    /// so all of them are scored again.
    /// </summary>
    /// <param name="id">The estimate identifier.</param>
    /// <param name="request">The new amounts of the estimate.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The estimate identifier (200), or a not found (404) without user settings or when the user has no such
    /// estimate.
    /// </returns>
    [WolverinePut(
        "/financial-wellness/v1/snapshots/estimates/{id}",
        OperationId = "UpdateEstimatedSnapshot"
    )]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<Guid>> Put(
        Guid id,
        EstimateRequest request,
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
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The user settings do not exist.");

        Guid userId = userSettings.UserId;
        List<EstimatedSnapshot> estimates = await db
            .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
            .ToListAsync(ct);
        EstimatedSnapshot? estimate = estimates.SingleOrDefault(stored => stored.Id == id);

        if (estimate is null)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The estimate does not exist.");

        DateTime now = time.GetUtcNow().UtcDateTime;

        estimate.ExpensesCurrency = request.ExpensesTotal.Currency;
        estimate.ExpensesAmount = request.ExpensesTotal.Amount;
        estimate.SavedCurrency = request.Saved.Currency;
        estimate.SavedAmount = request.Saved.Amount;
        estimate.UpdatedAt = now;

        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userId)
            .ToListAsync(ct);

        Dictionary<SnapshotMonth, SnapshotScore> scores = await SnapshotRescorer.ScoreAsync(
            db,
            estimates,
            snapshots,
            userSettings.BaseCurrency,
            ct
        );

        SnapshotRescorer.ApplyFrom(
            snapshots,
            SnapshotHistoryMapping.MonthOf(estimate),
            scores,
            now
        );

        return Response<Guid>.Ok(estimate.Id);
    }
}
