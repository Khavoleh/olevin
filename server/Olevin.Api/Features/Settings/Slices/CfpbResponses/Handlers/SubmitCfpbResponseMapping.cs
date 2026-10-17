namespace Olevin.Api.Features.Settings.Slices.CfpbResponses.Handlers;

/// <summary>
/// Codes the chosen options to response values.
/// </summary>
public static class SubmitCfpbResponseMapping
{
    private const int MostFavourable = 4;

    /// <summary>
    /// Codes the chosen options to response values (0-4, 4 is the most favourable), in the order of the statements.
    /// Statements 3, 5, 6, 7, 9 and 10 are worded negatively, so their coding is reversed.
    /// </summary>
    /// <param name="request">The validated request.</param>
    /// <returns>The 10 response values.</returns>
    public static short[] ToAnswers(SubmitCfpbResponseRequest request) =>
        [
            Positive(request.CouldHandleMajorExpense),
            Positive(request.SecuringFinancialFuture),
            Negative(request.NeverHaveThingsWanted),
            Positive(request.EnjoyLifeManagingMoney),
            Negative(request.JustGettingBy),
            Negative(request.MoneyWontLast),
            Negative(request.GiftStrainsFinances),
            Positive(request.MoneyLeftOver),
            Negative(request.BehindWithFinances),
            Negative(request.FinancesControlLife),
        ];

    private static short Positive<TAnswer>(TAnswer answer)
        where TAnswer : struct, Enum => (short)(MostFavourable - Convert.ToInt32(answer));

    private static short Negative<TAnswer>(TAnswer answer)
        where TAnswer : struct, Enum => (short)Convert.ToInt32(answer);
}
