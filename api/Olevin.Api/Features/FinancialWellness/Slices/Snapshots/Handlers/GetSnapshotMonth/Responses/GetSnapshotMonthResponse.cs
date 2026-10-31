using System.Text.Json.Serialization;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshotMonth.Responses;

/// <summary>
/// The amounts of one month as entered; <c>kind</c> says whether it is a snapshot or an estimate.
/// </summary>
/// <param name="Id">The identifier of the snapshot or the estimate.</param>
/// <param name="LaterSnapshots">The number of snapshots after the month, scored again when it changes.</param>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(ActualMonthResponse), "Actual")]
[JsonDerivedType(typeof(EstimatedMonthResponse), "Estimated")]
public abstract record GetSnapshotMonthResponse(Guid Id, int LaterSnapshots);
