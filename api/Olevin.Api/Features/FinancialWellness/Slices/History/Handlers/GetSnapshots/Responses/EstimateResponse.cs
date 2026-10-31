using Olevin.Api.Features.FinancialWellness.Data;

namespace Olevin.Api.Features.FinancialWellness.Slices.History.Handlers.GetSnapshots.Responses;

/// <summary>
/// One estimate from onboarding.
/// </summary>
/// <param name="Id">The estimate identifier.</param>
/// <param name="Year">The year of the estimate.</param>
/// <param name="Month">The month of the estimate.</param>
/// <param name="Expenses">The approximate total expenses.</param>
/// <param name="Saved">The approximate saved amount.</param>
public sealed record EstimateResponse(
    Guid Id,
    short Year,
    CalendarMonth Month,
    double Expenses,
    double Saved
);
