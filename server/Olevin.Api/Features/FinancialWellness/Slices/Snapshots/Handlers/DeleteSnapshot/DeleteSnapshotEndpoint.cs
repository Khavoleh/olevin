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
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.DeleteSnapshot;

/// <summary>
/// Deletes a snapshot of the signed-in user.
/// </summary>
[Tags("Snapshots")]
public static class DeleteSnapshotEndpoint
{
    /// <summary>
    /// Handles <c>DELETE /financial-wellness/v1/snapshots/{id}</c>. The later snapshots are scored again without it;
    /// nothing is scored again when it is the latest one.
    /// </summary>
    /// <param name="id">The snapshot identifier.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// No content (204), a not found (404) without user settings or when the user has no such snapshot, or a
    /// conflict (409) for the only snapshot of the user.
    /// </returns>
    [WolverineDelete("/financial-wellness/v1/snapshots/{id}", OperationId = "DeleteSnapshot")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status409Conflict)]
    public static async Task<IResult> Delete(
        Guid id,
        ClaimsPrincipal principal,
        FinancialWellnessDbContext db,
        IMessageBus bus,
        TimeProvider time,
        ILogger logger,
        CancellationToken ct
    )
    {
        UserSettingsDto? userSettings = await bus.InvokeAsync<UserSettingsDto?>(
            new GetUserSettingsRequest(principal.GetSubject()),
            ct
        );

        if (userSettings is null)
            return Fail(HttpStatusCode.NotFound, "The user settings do not exist.");

        Guid userId = userSettings.UserId;
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userId)
            .ToListAsync(ct);
        Snapshot? snapshot = snapshots.SingleOrDefault(stored => stored.Id == id);

        if (snapshot is null)
            return Fail(HttpStatusCode.NotFound, "The snapshot does not exist.");

        if (snapshots.Count == 1)
            return Fail(HttpStatusCode.Conflict, "The only snapshot cannot be deleted.");

        SnapshotMonth month = SnapshotHistoryMapping.MonthOf(snapshot);

        snapshots.Remove(snapshot);
        db.Snapshots.Remove(snapshot);

        logger.LogInformation(
            "Deleted the snapshot {SnapshotId} of the user {UserId}.",
            id,
            userId
        );

        if (DeleteSnapshotRules.IsLatest(month, snapshots.Select(SnapshotHistoryMapping.MonthOf)))
            return Results.NoContent();

        List<EstimatedSnapshot> estimates = await db
            .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
            .ToListAsync(ct);

        Dictionary<SnapshotMonth, SnapshotScore> scores = await SnapshotRescorer.ScoreAsync(
            db,
            estimates,
            snapshots,
            userSettings.BaseCurrency,
            ct
        );

        SnapshotRescorer.ApplyFrom(snapshots, month, scores, time.GetUtcNow().UtcDateTime);

        return Results.NoContent();
    }

    private static IResult Fail(HttpStatusCode code, string error) =>
        Results.Json(Response<Guid>.Fail(code, error), statusCode: (int)code);
}
