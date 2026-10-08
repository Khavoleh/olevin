using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Api.Features.Snapshots.Changes;

/// <summary>
/// Explains the change of I_fuzzy since the previous actual snapshot: each indicator of the previous snapshot is
/// replaced in turn by its current value.
/// </summary>
public static class ChangeAttribution
{
    /// <summary>
    /// Splits the change of I_fuzzy into contributions of the indicators and their joint effect.
    /// </summary>
    /// <param name="previous">The indicators of the previous actual snapshot.</param>
    /// <param name="current">The indicators of this snapshot.</param>
    /// <returns>The contributions; together with the joint effect they add up to the actual change.</returns>
    public static ChangeBreakdown Explain(IndicatorVector previous, IndicatorVector current)
    {
        IndicatorVector before = previous.Clamp();
        IndicatorVector after = current.Clamp();

        double from = OlevinModel.Evaluate(before).Value;
        double to = OlevinModel.Evaluate(after).Value;

        IndicatorContribution[] contributions =
        [
            .. Enum.GetValues<Indicator>()
                .Select(indicator => new IndicatorContribution(
                    indicator,
                    OlevinModel.Evaluate(before.With(indicator, after[indicator])).Value - from
                )),
        ];

        return new ChangeBreakdown(
            from,
            to,
            contributions,
            to - from - contributions.Sum(c => c.Contribution)
        );
    }
}
