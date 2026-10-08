using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Shared.Index.Model;

/// <summary>
/// The linear weighted index used as the baseline for the fuzzy index. It is stored but not shown to the user.
/// </summary>
public static class LinearIndex
{
    /// <summary>
    /// The reference points and weights: z = 0 at <c>Worst</c>, z = 1 at <c>Best</c>.
    /// For DTI and CV <c>Best</c> is below <c>Worst</c>, so z = 1 is always the best state.
    /// </summary>
    private static readonly Dictionary<
        Indicator,
        (double Worst, double Best, double Weight)
    > Points = new()
    {
        [Indicator.SavingsRate] = (0, 0.2, 0.25),
        [Indicator.Reserve] = (1, 6, 0.25),
        [Indicator.DebtToIncome] = (0.43, 0.2, 0.20),
        [Indicator.ExpenseVariation] = (0.3, 0.1, 0.15),
        [Indicator.SavingsRegularity] = (0.3, 0.8, 0.15),
    };

    /// <summary>
    /// Calculates the linear index.
    /// </summary>
    /// <param name="indicators">The indicators.</param>
    /// <returns>The index from 0 to 100.</returns>
    public static double Calculate(IndicatorVector indicators)
    {
        IndicatorVector x = indicators.Clamp();
        double sum = 0;

        foreach ((Indicator indicator, (double _, double _, double weight)) in Points)
            sum += weight * Normalize(indicator, x[indicator]);

        return 100 * sum;
    }

    /// <summary>
    /// Normalizes an indicator to [0; 1], where 1 is the best state.
    /// </summary>
    /// <param name="indicator">The indicator.</param>
    /// <param name="value">Its value.</param>
    /// <returns>z from 0 to 1.</returns>
    private static double Normalize(Indicator indicator, double value)
    {
        (double worst, double best, double _) = Points[indicator];

        return Math.Clamp((value - worst) / (best - worst), 0, 1);
    }
}
