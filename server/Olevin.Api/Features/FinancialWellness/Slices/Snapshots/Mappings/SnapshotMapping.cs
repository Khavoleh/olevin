using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;

/// <summary>
/// Creates the stored <see cref="Snapshot"/> from the amounts and the score of the core.
/// </summary>
public static class SnapshotMapping
{
    /// <summary>
    /// Creates a new snapshot with what the user has seen: the clamped indicators, the subsystems and the indexes.
    /// </summary>
    /// <param name="userId">The owning user identifier.</param>
    /// <param name="month">The month of the snapshot.</param>
    /// <param name="fxDate">The date of the rates the amounts were converted at.</param>
    /// <param name="amounts">The amounts as entered.</param>
    /// <param name="selfRating">The self-rating of calm from 1 to 5.</param>
    /// <param name="score">The score of the snapshot.</param>
    /// <returns>The snapshot with a new identifier.</returns>
    public static Snapshot ToSnapshot(
        Guid userId,
        SnapshotMonth month,
        DateOnly fxDate,
        SnapshotAmounts amounts,
        int selfRating,
        SnapshotScore score
    )
    {
        IndicatorVector indicators = score.Inputs.Indicators;
        IndexResult index = score.Index;

        return new Snapshot
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Month = month.Month,
            Year = month.Year,
            FxDate = fxDate,
            SelfRating = (short)selfRating,
            SavingsRate = indicators.SavingsRate,
            ReserveMonths = indicators.Reserve,
            DebtToIncome = indicators.DebtToIncome,
            ExpenseVariation = indicators.ExpenseVariation,
            SavingsRegularity = indicators.SavingsRegularity,
            CashFlow = index.CashFlow,
            Buffer = index.Buffer,
            FuzzyIndex = index.FuzzyIndex,
            LinearIndex = index.Linear,
            FinalIndex = index.Final,
            Amounts = amounts,
            ModelVersion = score.ModelVersion,
        };
    }
}
