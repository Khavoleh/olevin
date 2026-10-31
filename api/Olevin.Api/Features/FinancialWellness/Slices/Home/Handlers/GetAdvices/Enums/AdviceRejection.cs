namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Enums;

/// <summary>
/// Why an advice is not given.
/// </summary>
public enum AdviceRejection
{
    /// <summary>
    /// The indicator already has degree 1 in its best term, for example DTI ≤ 0.2 without debts.
    /// </summary>
    BestTerm,

    /// <summary>
    /// The step does not raise the shown index: no rule changes, a local dip of the centroid, no surplus to save or
    /// the final index is already 100.
    /// </summary>
    NoGain,
}
