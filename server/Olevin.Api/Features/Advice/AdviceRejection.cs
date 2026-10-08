namespace Olevin.Api.Features.Advice;

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
    /// The step does not raise I_fuzzy: no rule changes, a local dip of the centroid or no surplus to save.
    /// </summary>
    NoGain,
}
