using Olevin.Api.Features.FinancialWellness.Data;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.Responses;

/// <summary>
/// The change of the index since the previous snapshot.
/// </summary>
/// <param name="Year">The year of the previous snapshot.</param>
/// <param name="Month">The month of the previous snapshot.</param>
/// <param name="Delta">The difference of the rounded indexes, negative when the index fell.</param>
public sealed record IndexChangeResponse(short Year, CalendarMonth Month, double Delta);
