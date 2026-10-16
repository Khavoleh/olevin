using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;

/// <summary>
/// Maps the request to the stored amounts.
/// </summary>
public static class CreateSnapshotMapping
{
    /// <summary>
    /// Creates the stored amounts of the snapshot.
    /// </summary>
    /// <param name="request">The validated request.</param>
    /// <returns>The amounts as they are stored.</returns>
    public static SnapshotAmounts ToAmounts(CreateSnapshotRequest request) =>
        SnapshotAmountsMapping.ToAmounts(
            request.Income,
            request.Expenses,
            request.Saved,
            request.Reserves,
            request.DebtPayments
        );
}
