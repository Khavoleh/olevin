using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Features.Advice;

/// <summary>
/// The result of one step of the sensitivity analysis.
/// </summary>
/// <param name="Current">The indicators before the step, limited to their ranges.</param>
/// <param name="Changed">The indicators after the step.</param>
/// <param name="FuzzyGain">ΔI_fuzzy.</param>
/// <param name="LinearGain">ΔI_lin.</param>
public sealed record SensitivityResult(
    IndicatorVector Current,
    IndicatorVector Changed,
    double FuzzyGain,
    double LinearGain
);
