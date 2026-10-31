using Olevin.Api.Features.FinancialWellness.Data;

namespace Olevin.Api.Features.FinancialWellness.Slices.Trend.Handlers;

/// <summary>
/// One month of the trend.
/// </summary>
/// <param name="Year">The year of the month.</param>
/// <param name="Month">The month.</param>
/// <param name="Index">The final index rounded to one decimal, or <see langword="null"/> for a month without a snapshot.</param>
/// <param name="CashFlow">The cash flow subsystem rounded to one decimal, or <see langword="null"/> for a month without a snapshot.</param>
/// <param name="Buffer">The financial buffer subsystem rounded to one decimal, or <see langword="null"/> for a month without a snapshot.</param>
public sealed record TrendPointResponse(
    short Year,
    CalendarMonth Month,
    double? Index,
    double? CashFlow,
    double? Buffer
);
