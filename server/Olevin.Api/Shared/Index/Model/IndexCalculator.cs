using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Shared.Index.Model;

/// <summary>
/// Calculates the indexes of one snapshot from its inputs.
/// </summary>
public static class IndexCalculator
{
    /// <summary>
    /// Calculates I_fuzzy, I_lin, the final index and the zones.
    /// </summary>
    /// <param name="indicators">The inputs.</param>
    /// <param name="selfRating">The self-rating from 1 to 5.</param>
    /// <param name="actualSnapshots">The number of actual (not estimated) snapshots up to this one.</param>
    /// <returns>The indexes.</returns>
    public static IndexResult Calculate(
        IndicatorVector indicators,
        int selfRating,
        int actualSnapshots
    )
    {
        FuzzyEvaluation fuzzy = OlevinModel.Evaluate(indicators);
        double linear = LinearIndex.Calculate(indicators);
        double final = AdjustForSelfRating(fuzzy.Value, selfRating);

        return new IndexResult(
            fuzzy,
            linear,
            final,
            actualSnapshots < IndexConstants.FinalFromSnapshots
        );
    }

    /// <summary>
    /// Corrects the fuzzy index by the self-rating: ±2.5 points per step from the neutral 3, at most ±5.
    /// </summary>
    /// <param name="fuzzyIndex">I_fuzzy.</param>
    /// <param name="selfRating">The self-rating from 1 to 5, validated with the snapshot.</param>
    /// <returns>The final index I from 0 to 100.</returns>
    public static double AdjustForSelfRating(double fuzzyIndex, int selfRating)
    {
        double correction =
            IndexConstants.MaxSelfRatingCorrection
            * (selfRating - IndexConstants.NeutralSelfRating)
            / 2;

        return Math.Clamp(fuzzyIndex + correction, 0, 100);
    }
}
