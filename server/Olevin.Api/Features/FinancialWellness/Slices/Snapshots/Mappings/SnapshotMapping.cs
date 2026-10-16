using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;

/// <summary>
/// Creates and scores the stored <see cref="Snapshot"/>.
/// </summary>
public static class SnapshotMapping
{
    /// <summary>
    /// Creates a new snapshot with the entered values only; <see cref="ApplyScore"/> fills the calculated ones.
    /// </summary>
    /// <param name="userId">The owning user identifier.</param>
    /// <param name="month">The month of the snapshot.</param>
    /// <param name="fxDate">The date of the rates the amounts were converted at.</param>
    /// <param name="amounts">The amounts as entered.</param>
    /// <param name="selfRating">The self-rating of calm from 1 to 5.</param>
    /// <returns>The snapshot with a new identifier.</returns>
    public static Snapshot ToSnapshot(
        Guid userId,
        SnapshotMonth month,
        DateOnly fxDate,
        SnapshotAmounts amounts,
        int selfRating
    ) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Month = month.Month,
            Year = month.Year,
            FxDate = fxDate,
            SelfRating = (short)selfRating,
            Amounts = amounts,
        };

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
