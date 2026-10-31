namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// Save something in one more month.
/// </summary>
/// <param name="Gain">The rise of the final index in points, rounded to one decimal.</param>
/// <param name="SavingMonths">The number of recent months with savings now; the advice adds one.</param>
/// <param name="Months">The number of recent months the regularity covers.</param>
public sealed record SavingsRegularityAdviceResponse(double Gain, int SavingMonths, int Months)
    : AdviceResponse(Gain);
