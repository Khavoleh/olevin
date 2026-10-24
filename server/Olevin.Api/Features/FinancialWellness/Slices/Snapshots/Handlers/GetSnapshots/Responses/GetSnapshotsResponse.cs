using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshots.Responses;

/// <summary>
/// The snapshot history of the user.
/// </summary>
/// <param name="Currency">The base currency of every amount.</param>
/// <param name="Snapshots">Every snapshot, the latest first.</param>
/// <param name="Estimates">The estimates from onboarding, the latest first; they can be changed, not deleted.</param>
/// <param name="NewSnapshotOpensOn">
/// The day the next month opens for a snapshot, or <see langword="null"/> when a month is open now.
/// </param>
public sealed record GetSnapshotsResponse(
    CurrencyCode Currency,
    IReadOnlyList<SnapshotResponse> Snapshots,
    IReadOnlyList<EstimateResponse> Estimates,
    DateOnly? NewSnapshotOpensOn
);
