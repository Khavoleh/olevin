using Olevin.Api.Features.Snapshots.Currency;

namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// The results for all snapshots.
/// </summary>
/// <param name="ModelVersion">The version of the model.</param>
/// <param name="BaseCurrency">The currency of all amounts in the advice.</param>
/// <param name="Snapshots">The snapshots in month order.</param>
public sealed record CalculateSnapshotsResponse(
    string ModelVersion,
    CurrencyCode BaseCurrency,
    IReadOnlyList<SnapshotResponse> Snapshots
);
