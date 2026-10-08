namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// The result for one snapshot.
/// </summary>
/// <param name="Month">The first day of the month.</param>
/// <param name="IsEstimated">Whether this is an estimate from onboarding.</param>
/// <param name="Score">The score; <see langword="null"/> for an estimate, which has no index.</param>
public sealed record SnapshotResponse(DateOnly Month, bool IsEstimated, ScoreResponse? Score);
