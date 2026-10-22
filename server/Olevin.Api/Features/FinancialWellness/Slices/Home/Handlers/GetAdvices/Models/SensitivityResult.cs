using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;

/// <summary>
/// The result of one step of the sensitivity analysis.
/// </summary>
/// <param name="Current">The indicators before the step, limited to their ranges.</param>
/// <param name="Changed">The indicators after the step.</param>
/// <param name="FuzzyGain">ΔI_fuzzy.</param>
public sealed record SensitivityResult(
    IndicatorVector Current,
    IndicatorVector Changed,
    double FuzzyGain
);
