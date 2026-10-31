namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.HasFirstSnapshot;

/// <summary>
/// Asks whether the user has the first snapshot with the estimates of the 2 months before it.
/// </summary>
/// <param name="UserId">The identifier of the user.</param>
public sealed record HasFirstSnapshotRequest(Guid UserId);
