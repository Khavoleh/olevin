using Olevin.Api.Features.Snapshots.Changes;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// Everything calculated for an actual snapshot.
/// </summary>
/// <param name="Inputs">The model inputs and the values they come from.</param>
/// <param name="CashFlow">The "cash flow" subsystem from 0 to 100.</param>
/// <param name="Buffer">The "financial buffer" subsystem from 0 to 100.</param>
/// <param name="FuzzyIndex">I_fuzzy.</param>
/// <param name="LinearIndex">I_lin; for research only, not shown to the user.</param>
/// <param name="Index">The final index I, corrected by the self-rating.</param>
/// <param name="Zone">The zone of I.</param>
/// <param name="FuzzyZone">The zone of I_fuzzy.</param>
/// <param name="LinearZone">The zone of I_lin.</param>
/// <param name="IsPreliminary">Whether there are fewer than 3 actual snapshots up to this one.</param>
/// <param name="Change">The change of I_fuzzy since the previous actual snapshot.</param>
/// <param name="ModelVersion">The version of the model.</param>
public sealed record ScoreResponse(
    CalculatedInputs Inputs,
    double CashFlow,
    double Buffer,
    double FuzzyIndex,
    double LinearIndex,
    double Index,
    Zone Zone,
    Zone FuzzyZone,
    Zone LinearZone,
    bool IsPreliminary,
    ChangeBreakdown? Change,
    string ModelVersion
);
