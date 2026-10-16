using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.UpdateSnapshot;

/// <summary>
/// Maps the request to the stored amounts.
/// </summary>
public static class UpdateSnapshotMapping
{
    /// <summary>
    /// Creates the new stored amounts of the snapshot.
    /// </summary>
    /// <param name="request">The validated request.</param>
    /// <returns>The amounts as they are stored.</returns>
    public static SnapshotAmounts ToAmounts(UpdateSnapshotRequest request) =>
        SnapshotAmountsMapping.ToAmounts(
            request.Income,
            request.Expenses,
            request.Saved,
            request.Reserves,
            request.DebtPayments
        );
}
