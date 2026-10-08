using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Api.Features.Advice;

/// <summary>
/// Local sensitivity analysis: how much the indexes change when one indicator improves by a step and the rest stay
/// the same.
/// </summary>
public static class SensitivityAnalyzer
{
    /// <summary>
    /// Changes one indicator by a step and limits it to its range. A DTI step also raises SR by the same share,
    /// because the released payments stay with the person.
    /// </summary>
    /// <param name="indicators">The indicators.</param>
    /// <param name="indicator">The indicator to change.</param>
    /// <param name="step">The step δ_k; negative for DTI and CV.</param>
    /// <returns>The changed indicators.</returns>
    public static IndicatorVector Apply(
        IndicatorVector indicators,
        Indicator indicator,
        double step
    )
    {
        IndicatorVector changed = Shift(indicators.Clamp(), indicator, step);

        return indicator == Indicator.DebtToIncome
            ? Shift(changed, Indicator.SavingsRate, -step)
            : changed;
    }

    /// <summary>
    /// Calculates the gains of both indexes for a step of one indicator.
    /// </summary>
    /// <param name="indicators">The indicators.</param>
    /// <param name="indicator">The indicator to change.</param>
    /// <param name="step">The step δ_k.</param>
    /// <returns>The changed indicators and the gains ΔI_fuzzy and ΔI_lin.</returns>
    public static SensitivityResult Analyze(
        IndicatorVector indicators,
        Indicator indicator,
        double step
    )
    {
        IndicatorVector current = indicators.Clamp();
        IndicatorVector changed = Apply(current, indicator, step);

        return new SensitivityResult(
            current,
            changed,
            OlevinModel.Evaluate(changed).Value - OlevinModel.Evaluate(current).Value,
            LinearIndex.Calculate(changed) - LinearIndex.Calculate(current)
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
