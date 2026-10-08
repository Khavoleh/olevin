using Olevin.Api.Features.Snapshots.Changes;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Api.Features.Snapshots;

/// <summary>
/// Everything calculated for one actual snapshot.
/// </summary>
/// <param name="Inputs">The model inputs.</param>
/// <param name="Index">The indexes and the zone.</param>
/// <param name="Change">The change since the previous actual snapshot; <see langword="null"/> for the first one.</param>
/// <param name="ModelVersion">The version of the model that produced the score.</param>
public sealed record SnapshotScore(
    CalculatedInputs Inputs,
    IndexResult Index,
    ChangeBreakdown? Change,
    string ModelVersion
);
