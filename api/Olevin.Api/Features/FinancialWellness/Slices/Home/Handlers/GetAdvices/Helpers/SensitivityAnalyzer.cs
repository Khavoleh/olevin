using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;

/// <summary>
/// Local sensitivity analysis: how much I_fuzzy changes when one indicator improves by a step and the rest stay
/// the same.
/// </summary>
public static class SensitivityAnalyzer
{
    /// <summary>
    /// Changes one indicator by a step from the given value, then limits every indicator to its range. A DTI step
    /// also raises SR by the same share, because the released payments stay with the person.
    /// </summary>
    /// <param name="indicators">The indicators, limited or raw.</param>
    /// <param name="indicator">The indicator to change.</param>
    /// <param name="step">The step δ_k; negative for DTI and CV.</param>
    /// <returns>The changed indicators.</returns>
    public static IndicatorVector Apply(
        IndicatorVector indicators,
        Indicator indicator,
        double step
    )
    {
        IndicatorVector changed = Shift(indicators, indicator, step);

        return indicator == Indicator.DebtToIncome
            ? Shift(changed, Indicator.SavingsRate, -step).Clamp()
            : changed.Clamp();
    }

    /// <summary>
    /// Calculates the gain of I_fuzzy for a step of one indicator.
    /// </summary>
    /// <param name="indicators">The indicators, limited or raw; the step starts from these values.</param>
    /// <param name="indicator">The indicator to change.</param>
    /// <param name="step">The step δ_k.</param>
    /// <returns>The changed indicators and the gain ΔI_fuzzy.</returns>
    public static SensitivityResult Analyze(
        IndicatorVector indicators,
        Indicator indicator,
        double step
    )
    {
        IndicatorVector current = indicators.Clamp();
        IndicatorVector changed = Apply(indicators, indicator, step);

        return new SensitivityResult(
            current,
            changed,
            OlevinModel.Evaluate(changed).Value - OlevinModel.Evaluate(current).Value
        );
    }

    private static IndicatorVector Shift(
        IndicatorVector indicators,
        Indicator indicator,
        double step
    )
    {
        double value = OlevinModel.Variable(indicator).Clamp(indicators[indicator] + step);

        return indicators.With(indicator, value);
    }
}
