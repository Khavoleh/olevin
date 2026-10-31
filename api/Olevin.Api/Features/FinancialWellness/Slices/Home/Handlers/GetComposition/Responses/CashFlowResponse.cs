using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Enums;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Responses;

/// <summary>
/// The "cash flow" subsystem and the indicators it is calculated from.
/// </summary>
/// <param name="Value">The output from 0 to 100, rounded to one decimal.</param>
/// <param name="Term">The term of <paramref name="Value"/>.</param>
/// <param name="SavingsRate">The savings rate SR before limiting, for example −0.35; <see langword="null"/> without income.</param>
/// <param name="DebtToIncome">The debt-to-income DTI before limiting; <see langword="null"/> without income.</param>
public sealed record CashFlowResponse(
    double Value,
    SubsystemTerm Term,
    double? SavingsRate,
    double? DebtToIncome
);
