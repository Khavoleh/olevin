using Olevin.Api.Features.Snapshots.Changes;
using Olevin.Api.Shared.Index;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Api.Features.Snapshots;

/// <summary>
/// Runs the whole calculation for the snapshots of one user: inputs, indexes and the change since the
/// previous actual snapshot. R, CV and RG depend on earlier months, so a change of any snapshot means scoring all
/// the following ones again; for the same amounts and model version the results are always the same.
/// </summary>
public static class SnapshotScorer
{
    /// <summary>
    /// Scores every actual snapshot.
    /// </summary>
    /// <param name="months">All snapshots in month order, estimates included, in the base currency.</param>
    /// <returns>The score of every snapshot in the same order; <see langword="null"/> for estimates.</returns>
    public static IReadOnlyList<SnapshotScore?> Score(IReadOnlyList<MonthFigures> months)
    {
        SnapshotScore?[] scores = new SnapshotScore?[months.Count];
        IndicatorVector? previous = null;
        int actual = 0;

        for (int i = 0; i < months.Count; i++)
        {
            MonthFigures month = months[i];

            if (month.IsEstimated)
                continue;

            actual++;

            CalculatedInputs inputs = InputsCalculator.Calculate(months, i);
            IndexResult index = IndexCalculator.Calculate(
                inputs.Indicators,
                month.SelfRating,
                actual
            );
            ChangeBreakdown? change = previous is null
                ? null
                : ChangeAttribution.Explain(previous, inputs.Indicators);

            scores[i] = new SnapshotScore(inputs, index, change, IndexConstants.ModelVersion);
            previous = inputs.Indicators;
        }

        return scores;
    }
}
