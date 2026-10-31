using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.Scoring;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;

/// <summary>
/// Creates the stored <see cref="Snapshot"/>.
/// </summary>
public static class SnapshotMapping
{
    /// <summary>
    /// Creates a new snapshot with the entered values only; <see cref="SnapshotScoreMapping.ApplyScore"/> fills the
    /// calculated ones.
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
}
