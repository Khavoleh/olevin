using Olevin.Api.Features.FinancialWellness.Data;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetOpenMonths.Responses;

/// <summary>
/// A month a new snapshot may be created for.
/// </summary>
/// <param name="Year">The year.</param>
/// <param name="Month">The month.</param>
public sealed record OpenMonthResponse(short Year, CalendarMonth Month);
