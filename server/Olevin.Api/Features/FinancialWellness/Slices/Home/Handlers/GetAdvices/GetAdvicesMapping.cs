using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Enums;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices;

/// <summary>
/// Turns the advice analysis into the advice shown to the user.
/// </summary>
public static class GetAdvicesMapping
{
    /// <summary>
    /// Picks the shown advice.
    /// </summary>
    /// <param name="advice">All five rows of the analysis.</param>
    /// <param name="inputs">The inputs of the snapshot the advice is for.</param>
    /// <returns>The advice in the top three, the largest gain first.</returns>
    public static IReadOnlyList<AdviceResponse> ToResponses(
        IReadOnlyList<AdviceItem> advice,
        CalculatedInputs inputs
    ) =>
        [
            .. advice
                .Where(item => item.IsShown)
                .OrderBy(item => item.Rank)
                .Select(item => ToResponse(item, inputs)),
        ];

    /// <summary>
    /// Finds whether there is advice, and why not when there is none.
    /// </summary>
    /// <param name="advice">All five rows of the analysis.</param>
    /// <param name="finalIndex">The final index of the snapshot the advice is for.</param>
    /// <returns>The status of the advice.</returns>
    public static AdviceStatus StatusOf(IReadOnlyList<AdviceItem> advice, double finalIndex) =>
        (
            advice.Any(item => item.IsShown),
            finalIndex >= 100 - AdviceConstants.Tolerance,
            advice.All(item => item.Rejection == AdviceRejection.BestTerm)
        ) switch
        {
            (true, _, _) => AdviceStatus.Shown,
            (_, true, _) => AdviceStatus.AtMaximum,
            (_, _, true) => AdviceStatus.AllBest,
            _ => AdviceStatus.NoQuickWin,
        };

    /// <summary>
    /// Turns one row of the analysis into the advice of its indicator.
    /// </summary>
    /// <param name="item">The row.</param>
    /// <param name="inputs">The inputs of the snapshot the advice is for.</param>
    /// <returns>The advice with the gain of the final index rounded to one decimal and the amounts to cents.</returns>
    public static AdviceResponse ToResponse(AdviceItem item, CalculatedInputs inputs)
    {
        double gain = Math.Round(item.FinalGain, 1, MidpointRounding.AwayFromZero);
        double amount = Money(item.Amount ?? 0);

        IndicatorChangeResponse Change(Indicator indicator)
        {
            IndicatorChange change = item.Changes.Single(change => change.Indicator == indicator);

            return new IndicatorChangeResponse(change.From, change.To);
        }

        return item.Indicator switch
        {
            Indicator.SavingsRate => new SavingsRateAdviceResponse(
                gain,
                Change(Indicator.SavingsRate),
                amount,
                item.IsPartial,
                [
                    .. item.Cuts.Select(cut => new ExpenseCutResponse(
                        cut.Category,
                        Money(cut.Amount)
                    )),
                ]
            ),
            Indicator.Reserve => new ReserveAdviceResponse(
                gain,
                Change(Indicator.Reserve),
                Money((item.Amount ?? 0) / AdviceConstants.ReserveTopUpMonths),
                AdviceConstants.ReserveTopUpMonths
            ),
            Indicator.DebtToIncome => new DebtToIncomeAdviceResponse(
                gain,
                Change(Indicator.DebtToIncome),
                Change(Indicator.SavingsRate),
                amount
            ),
            Indicator.ExpenseVariation => new ExpenseVariationAdviceResponse(
                gain,
                Change(Indicator.ExpenseVariation),
                amount
            ),
            Indicator.SavingsRegularity => new SavingsRegularityAdviceResponse(
                gain,
                inputs.SavingMonths,
                inputs.HistoryMonths
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(item), item.Indicator, null),
        };
    }

    private static double Money(double amount) =>
        Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}
