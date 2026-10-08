using Olevin.Api.Features.Snapshots.Currency;

namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// The snapshots of one user.
/// </summary>
/// <param name="BaseCurrency">The currency in which all amounts are calculated.</param>
/// <param name="Snapshots">The snapshots in any order, estimates from onboarding included.</param>
public sealed record CalculateSnapshotsRequest(
    CurrencyCode BaseCurrency,
    IReadOnlyList<SnapshotRequest> Snapshots
);
