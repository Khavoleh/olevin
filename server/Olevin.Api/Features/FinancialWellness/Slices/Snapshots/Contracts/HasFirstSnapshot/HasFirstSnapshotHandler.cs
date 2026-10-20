using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Wolverine.Attributes;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.HasFirstSnapshot;

/// <summary>
/// Tells other features whether the user has the first snapshot.
/// </summary>
public static class HasFirstSnapshotHandler
{
    /// <summary>
    /// Handles <see cref="HasFirstSnapshotRequest"/>.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> when the user has at least one snapshot and the estimates
    /// </returns>
    /// <remarks>It only reads, so it opts out of the automatic transactions.</remarks>
    [NonTransactional]
    public static async Task<bool> Handle(
        HasFirstSnapshotRequest request,
        FinancialWellnessDbContext db,
        CancellationToken ct
    ) =>
        await db.Snapshots.AnyAsync(snapshot => snapshot.UserId == request.UserId, ct)
        && await db.EstimatedSnapshots.AnyAsync(estimate => estimate.UserId == request.UserId, ct);
}
