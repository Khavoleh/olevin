using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;

namespace Olevin.Api.Features.FinancialWellness.Helpers.Scoring;

/// <summary>
/// Everything calculated for one actual snapshot.
/// </summary>
/// <param name="Month">The month of the snapshot.</param>
/// <param name="Inputs">The model inputs.</param>
/// <param name="Index">The indexes and the zone.</param>
/// <param name="ModelVersion">The version of the model that produced the score.</param>
public sealed record SnapshotScore(
    SnapshotMonth Month,
    CalculatedInputs Inputs,
    IndexResult Index,
    string ModelVersion
);
