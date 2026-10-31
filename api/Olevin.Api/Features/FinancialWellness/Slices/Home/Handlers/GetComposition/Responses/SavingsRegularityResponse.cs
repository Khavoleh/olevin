using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Enums;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Responses;

/// <summary>
/// The savings regularity: in which of the recent months the user saved something.
/// </summary>
/// <param name="Term">The term of the share of <paramref name="Months"/> with savings.</param>
/// <param name="Months">From 1 to 6 recent months, estimates included, the oldest first.</param>
public sealed record SavingsRegularityResponse(
    SavingsRegularityTerm Term,
    IReadOnlyList<SavingsMonthResponse> Months
);
