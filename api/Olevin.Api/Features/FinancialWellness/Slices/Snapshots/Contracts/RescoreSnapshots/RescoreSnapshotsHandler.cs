using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.Scoring;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.RescoreSnapshots;

/// <summary>
/// Scores every snapshot of a user again after their base currency changes.
/// </summary>
public static class RescoreSnapshotsHandler
{
    /// <summary>
    /// Handles <see cref="RescoreSnapshotsRequest"/> from the outbox, after the new base currency is saved. Every month
    /// is converted to the new base currency at its stored rates.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that completes when the scores are applied.</returns>
    public static async Task Handle(
        RescoreSnapshotsRequest request,
        FinancialWellnessDbContext db,
        TimeProvider time,
        ILogger logger,
        CancellationToken ct
    )
    {
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == request.UserId)
            .ToListAsync(ct);

        if (snapshots.Count == 0)
            return;

        List<EstimatedSnapshot> estimates = await db
            .EstimatedSnapshots.Where(estimate => estimate.UserId == request.UserId)
            .ToListAsync(ct);

        Dictionary<SnapshotMonth, SnapshotScore> scores = await SnapshotRescorer.ScoreAsync(
            db,
            estimates,
            snapshots,
            request.BaseCurrency,
            ct
        );

        SnapshotRescorer.ApplyAll(snapshots, scores, time.GetUtcNow().UtcDateTime);

        logger.LogInformation(
            "Scored {Count} snapshots of the user {UserId} again in {BaseCurrency}.",
            snapshots.Count,
            request.UserId,
            request.BaseCurrency
        );
    }
}
