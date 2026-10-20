using Olevin.Api.Features.FinancialWellness.Data;

namespace Olevin.Api.Features.FinancialWellness.Slices.Index.Handlers;

/// <summary>
/// The columns of a stored snapshot the index needs.
/// </summary>
/// <param name="Year">The year of the snapshot.</param>
/// <param name="Month">The month of the snapshot.</param>
/// <param name="FinalIndex">The final index (0-100).</param>
public sealed record SnapshotDto(short Year, CalendarMonth Month, double FinalIndex);
