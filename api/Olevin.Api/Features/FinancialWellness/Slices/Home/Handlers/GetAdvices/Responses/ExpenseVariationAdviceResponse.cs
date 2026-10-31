namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// Keep the monthly expenses closer to their average.
/// </summary>
/// <param name="Gain">The rise of the final index in points, rounded to one decimal.</param>
/// <param name="ExpenseVariation">The variation of expenses now and after the advice.</param>
/// <param name="Amount">How much smaller the typical deviation of monthly expenses should be, rounded to cents.</param>
public sealed record ExpenseVariationAdviceResponse(
    double Gain,
    IndicatorChangeResponse ExpenseVariation,
    double Amount
) : AdviceResponse(Gain);
