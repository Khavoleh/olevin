using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Enums;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Responses;

/// <summary>
/// The "financial buffer" subsystem and the indicators it is calculated from.
/// </summary>
/// <param name="Value">The output from 0 to 100, rounded to one decimal.</param>
/// <param name="Term">The term of <paramref name="Value"/>.</param>
/// <param name="ReserveMonths">The reserve R in months before limiting; <see langword="null"/> without expenses and debt payments.</param>
/// <param name="ExpenseVariation">The expense variation CV before limiting; <see langword="null"/> while there is one month.</param>
public sealed record BufferResponse(
    double Value,
    SubsystemTerm Term,
    double? ReserveMonths,
    double? ExpenseVariation
);
