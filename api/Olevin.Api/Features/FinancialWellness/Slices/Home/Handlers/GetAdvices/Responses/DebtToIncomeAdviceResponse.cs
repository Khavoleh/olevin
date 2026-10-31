namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// Lower the monthly debt payments.
/// </summary>
/// <param name="Gain">The rise of the final index in points, rounded to one decimal.</param>
/// <param name="DebtToIncome">The debt-to-income now and after the cut.</param>
/// <param name="SavingsRate">The savings rate now and after the cut.</param>
/// <param name="Amount">The monthly cut of the debt payments, rounded to cents.</param>
public sealed record DebtToIncomeAdviceResponse(
    double Gain,
    IndicatorChangeResponse DebtToIncome,
    IndicatorChangeResponse SavingsRate,
    double Amount
) : AdviceResponse(Gain);
