using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;

namespace Olevin.Api.Features.FinancialWellness.Helpers.Scoring;

/// <summary>
/// Writes a score into the stored <see cref="Snapshot"/>.
/// </summary>
public static class SnapshotScoreMapping
{
    /// <summary>
    /// Replaces the calculated values of a snapshot with a new score.
    /// </summary>
    /// <param name="snapshot">The snapshot.</param>
    /// <param name="score">The new score.</param>
    public static void ApplyScore(Snapshot snapshot, SnapshotScore score)
    {
        IndicatorVector indicators = score.Inputs.Indicators;
        IndexResult index = score.Index;

        snapshot.SavingsRate = indicators.SavingsRate;
        snapshot.ReserveMonths = indicators.Reserve;
        snapshot.DebtToIncome = indicators.DebtToIncome;
        snapshot.ExpenseVariation = indicators.ExpenseVariation;
        snapshot.SavingsRegularity = indicators.SavingsRegularity;
        snapshot.CashFlow = index.CashFlow;
        snapshot.Buffer = index.Buffer;
        snapshot.FuzzyIndex = index.FuzzyIndex;
        snapshot.LinearIndex = index.Linear;
        snapshot.FinalIndex = index.Final;
        snapshot.ModelVersion = score.ModelVersion;
    }
}
