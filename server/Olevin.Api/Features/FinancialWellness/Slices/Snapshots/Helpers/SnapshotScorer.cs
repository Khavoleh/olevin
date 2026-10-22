using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;

/// <summary>
/// Runs the whole calculation for the snapshots of one user: inputs and indexes. R, CV and RG depend on earlier
/// months, so a change of any snapshot means scoring all the following ones again.
/// </summary>
public static class SnapshotScorer
{
    /// <summary>
    /// Scores every actual snapshot.
    /// </summary>
    /// <param name="history">All months in order, estimates included, in the base currency.</param>
    /// <returns>The scores of the actual snapshots in month order.</returns>
    public static IReadOnlyList<SnapshotScore> Score(IReadOnlyList<SnapshotFigures> history)
    {
        MonthFigures[] months = [.. history.Select(item => item.Figures)];
        List<SnapshotScore> scores = [];
        int actual = 0;

        for (int i = 0; i < months.Length; i++)
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

            scores.Add(
                new SnapshotScore(history[i].Month, inputs, index, IndexConstants.ModelVersion)
            );
        }

        return scores;
    }
}
