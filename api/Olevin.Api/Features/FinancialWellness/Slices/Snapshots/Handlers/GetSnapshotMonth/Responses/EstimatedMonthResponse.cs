using Olevin.Api.Features.FinancialWellness.Data.Snapshots;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshotMonth.Responses;

/// <summary>
/// The amounts of an estimate from onboarding as entered.
/// </summary>
/// <param name="Id">The estimate identifier.</param>
/// <param name="LaterSnapshots">The number of snapshots after the month, scored again when it changes.</param>
/// <param name="ExpensesTotal">The approximate total expenses of the month.</param>
/// <param name="Saved">The amount saved during the month.</param>
public sealed record EstimatedMonthResponse(
    Guid Id,
    int LaterSnapshots,
    Money ExpensesTotal,
    Money Saved
) : GetSnapshotMonthResponse(Id, LaterSnapshots);
