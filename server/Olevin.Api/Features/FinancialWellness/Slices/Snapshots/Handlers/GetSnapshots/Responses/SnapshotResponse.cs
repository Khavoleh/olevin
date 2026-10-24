using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshots.Responses;

/// <summary>
/// One snapshot of the history.
/// </summary>
/// <param name="Id">The snapshot identifier.</param>
/// <param name="Year">The year of the snapshot.</param>
/// <param name="Month">The month of the snapshot.</param>
/// <param name="Index">The final index, rounded to one decimal.</param>
/// <param name="Zone">The zone of <paramref name="Index"/>.</param>
/// <param name="Change">The change since the previous snapshot, or <see langword="null"/> for the first one.</param>
/// <param name="Expenses">The total expenses without debt payments.</param>
/// <param name="Saved">The amount saved during the month.</param>
public sealed record SnapshotResponse(
    Guid Id,
    short Year,
    CalendarMonth Month,
    double Index,
    Zone Zone,
    double? Change,
    double Expenses,
    double Saved
);
