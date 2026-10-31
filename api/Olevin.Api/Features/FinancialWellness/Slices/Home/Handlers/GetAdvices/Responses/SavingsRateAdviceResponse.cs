namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// Cut the flexible expenses to save more of the income.
/// </summary>
/// <param name="Gain">The rise of the final index in points, rounded to one decimal.</param>
/// <param name="SavingsRate">The savings rate now and after the cut.</param>
/// <param name="Amount">The monthly cut, rounded to cents.</param>
/// <param name="IsPartial">Whether the cut is all the flexible expenses and still smaller than 5% of the income.</param>
/// <param name="Cuts">How the cut splits between the flexible categories, the largest first.</param>
public sealed record SavingsRateAdviceResponse(
    double Gain,
    IndicatorChangeResponse SavingsRate,
    double Amount,
    bool IsPartial,
    IReadOnlyList<ExpenseCutResponse> Cuts
) : AdviceResponse(Gain);
