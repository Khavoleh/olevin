namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Responses;

/// <summary>
/// What the latest index of the user is made of: the two subsystems and the savings regularity.
/// </summary>
/// <param name="CashFlow">The "cash flow" subsystem F₁.</param>
/// <param name="Buffer">The "financial buffer" subsystem F₂.</param>
/// <param name="SavingsRegularity">The savings regularity RG.</param>
public sealed record GetCompositionResponse(
    CashFlowResponse CashFlow,
    BufferResponse Buffer,
    SavingsRegularityResponse SavingsRegularity
);
