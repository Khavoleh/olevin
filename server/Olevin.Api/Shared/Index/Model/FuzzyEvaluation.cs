using Olevin.Api.Shared.Index.Fuzzy;

namespace Olevin.Api.Shared.Index.Model;

/// <summary>
/// The results of the three subsystems of the model.
/// </summary>
/// <param name="CashFlow">F₁ "cash flow".</param>
/// <param name="Buffer">F₂ "financial buffer".</param>
/// <param name="Index">F₃, the final output.</param>
public sealed record FuzzyEvaluation(
    MamdaniResult CashFlow,
    MamdaniResult Buffer,
    MamdaniResult Index
)
{
    /// <summary>
    /// Gets the fuzzy index I_fuzzy from 0 to 100.
    /// </summary>
    public double Value => Index.Output;
}
