using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.UpdateSnapshot;

/// <summary>
/// Changes a snapshot of the signed-in user.
/// </summary>
[Tags("Snapshots")]
public static class UpdateSnapshotEndpoint
{
    /// <summary>
    /// Handles <c>PUT /financial-wellness/v1/snapshots/{id}</c>. The snapshot and the later ones are scored again;
    /// the month and the rate date of the snapshot never change.
    /// </summary>
    /// <param name="id">The snapshot identifier.</param>
    /// <param name="request">The new values of the snapshot.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The snapshot identifier (200), or a not found (404) without user settings or when the user has no such
    /// snapshot.
    /// </returns>
    [WolverinePut("/financial-wellness/v1/snapshots/{id}")]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<Guid>> Put(
        Guid id,
        UpdateSnapshotRequest request,
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
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userId)
            .ToListAsync(ct);
        Snapshot? snapshot = snapshots.SingleOrDefault(stored => stored.Id == id);

        if (snapshot is null)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The snapshot does not exist.");

        List<EstimatedSnapshot> estimates = await db
            .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
            .ToListAsync(ct);

        SnapshotMonth month = SnapshotHistoryMapping.MonthOf(snapshot);

        snapshot.Amounts = SnapshotAmountsMapping.ToAmounts(
            request.Income,
            request.Expenses,
            request.Saved,
            request.Reserves,
            request.DebtPayments
        );
        snapshot.SelfRating = (short)request.SelfRating;

        Dictionary<SnapshotMonth, SnapshotScore> scores = await SnapshotRescorer.ScoreAsync(
            db,
            estimates,
            snapshots,
            userSettings.BaseCurrency,
            ct
        );

        SnapshotRescorer.ApplyFrom(snapshots, month, scores, time.GetUtcNow().UtcDateTime);

        return Response<Guid>.Ok(snapshot.Id);
    }
}
