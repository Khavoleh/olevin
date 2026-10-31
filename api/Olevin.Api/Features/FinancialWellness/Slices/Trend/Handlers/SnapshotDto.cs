using Olevin.Api.Features.FinancialWellness.Data;

namespace Olevin.Api.Features.FinancialWellness.Slices.Trend.Handlers;

/// <summary>
/// The columns of a stored snapshot the trend needs.
/// </summary>
/// <param name="Year">The year of the snapshot.</param>
/// <param name="Month">The month of the snapshot.</param>
/// <param name="FinalIndex">The final index (0-100).</param>
/// <param name="CashFlow">The output of the cash flow subsystem (0-100).</param>
/// <param name="Buffer">The output of the financial buffer subsystem (0-100).</param>
public sealed record SnapshotDto(
    short Year,
    CalendarMonth Month,
    double FinalIndex,
    double CashFlow,
    double Buffer
);
