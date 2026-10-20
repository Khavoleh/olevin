namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.CreateFirstSnapshot;

/// <summary>
/// Asks to save the first snapshot of the user signed in with <paramref name="AuthSub"/>, together with the
/// estimates of the 2 months before it.
/// </summary>
/// <param name="AuthSub">The subject of the user's access token.</param>
/// <param name="Snapshot">The validated first snapshot.</param>
public sealed record CreateFirstSnapshotRequest(string AuthSub, FirstSnapshot Snapshot);
