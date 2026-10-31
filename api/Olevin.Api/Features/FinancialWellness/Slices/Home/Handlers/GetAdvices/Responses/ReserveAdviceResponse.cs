namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// Put the surplus into the reserve for a few months.
/// </summary>
/// <param name="Gain">The rise of the final index in points, rounded to one decimal.</param>
/// <param name="Reserve">The reserve in months of expenses and debt payments now and after the top-up.</param>
/// <param name="MonthlyAmount">The amount to put aside every month, rounded to cents.</param>
/// <param name="Months">The number of months to put it aside.</param>
public sealed record ReserveAdviceResponse(
    double Gain,
    IndicatorChangeResponse Reserve,
    double MonthlyAmount,
    int Months
) : AdviceResponse(Gain);
