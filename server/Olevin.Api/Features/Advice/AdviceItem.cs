using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Features.Advice;

/// <summary>
/// One row of the advice analysis: a step for one indicator, its cost and the index gain.
/// </summary>
/// <param name="Indicator">The indicator to improve.</param>
/// <param name="Step">The step δ_k actually analysed.</param>
/// <param name="Changes">The indicators the step changes, from and to.</param>
/// <param name="FuzzyGain">ΔI_fuzzy.</param>
/// <param name="LinearGain">ΔI_lin.</param>
/// <param name="Amount">
/// The step in money of the base currency: the monthly cut of expenses (SR), the top-up of reserves over three
/// months (R), the monthly cut of debt payments (DTI) or the cut of the standard deviation of expenses (CV);
/// <see langword="null"/> for RG.
/// </param>
/// <param name="IsPartial">Whether the flexible expenses only allow a smaller cut than the full SR step.</param>
/// <param name="Cuts">How the cut of expenses splits between flexible categories, largest first; SR only.</param>
/// <param name="Rank">The place among the advice that passed the filter; the first three are shown.</param>
/// <param name="Rejection">Why the advice was filtered out, if it was.</param>
public sealed record AdviceItem(
    Indicator Indicator,
    double Step,
    IReadOnlyList<IndicatorChange> Changes,
    double FuzzyGain,
    double LinearGain,
    double? Amount,
    bool IsPartial,
    IReadOnlyList<ExpenseCut> Cuts,
    int? Rank,
    AdviceRejection? Rejection
)
{
    /// <summary>
    /// Gets a value indicating whether the advice is in the top three shown to the user.
    /// </summary>
    public bool IsShown => Rank <= AdviceConstants.ShownAdvice;
}
