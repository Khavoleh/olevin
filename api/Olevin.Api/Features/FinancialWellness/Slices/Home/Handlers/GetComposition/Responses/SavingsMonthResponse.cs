using Olevin.Api.Features.FinancialWellness.Data;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Responses;

/// <summary>
/// One month of the savings regularity.
/// </summary>
/// <param name="Year">The year of the month.</param>
/// <param name="Month">The month.</param>
/// <param name="HasSaved">Whether the user saved something that month.</param>
public sealed record SavingsMonthResponse(short Year, CalendarMonth Month, bool HasSaved);
