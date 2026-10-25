namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots;

/// <summary>
/// Asks for every snapshot of the user.
/// </summary>
/// <param name="UserId">The identifier of the user.</param>
public sealed record GetAllSnapshotsRequest(Guid UserId);
