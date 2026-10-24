using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Fuzzy;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Enums;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Responses;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition;

/// <summary>
/// The rules of the composition of the index.
/// </summary>
public static class GetCompositionRules
{
    /// <summary>
    /// Gets the term of a subsystem output.
    /// </summary>
    /// <param name="value">The output of "cash flow" or "financial buffer" from 0 to 100.</param>
    /// <returns>The term with the highest degree, the weaker one on a tie.</returns>
    public static SubsystemTerm SubsystemTermOf(double value) =>
        (SubsystemTerm)StrongestTerm(OlevinModel.CashFlow, value);

    /// <summary>
    /// Gets the term of the savings regularity.
    /// </summary>
    /// <param name="regularity">The share of recent months with savings, RG.</param>
    /// <returns>The term with the highest degree, the rarer one on a tie.</returns>
    public static SavingsRegularityTerm SavingsRegularityTermOf(double regularity) =>
        (SavingsRegularityTerm)StrongestTerm(OlevinModel.SavingsRegularity, regularity);

    /// <summary>
    /// Gets the months the savings regularity covers.
    /// </summary>
    /// <param name="history">The history of the user in month order, estimates included.</param>
    /// <returns>The last <see cref="IndexConstants.HistoryWindow"/> months at most, the oldest first.</returns>
    public static IReadOnlyList<SavingsMonthResponse> SavingsMonths(
        IReadOnlyList<SnapshotFigures> history
    ) =>
        [
            .. history
                .TakeLast(IndexConstants.HistoryWindow)
                .Select(item => new SavingsMonthResponse(
                    item.Month.Year,
                    item.Month.Month,
                    item.Figures.Saved > 0
                )),
        ];

    private static int StrongestTerm(LinguisticVariable variable, double value)
    {
        double[] degrees = variable.Fuzzify(value);

        return Array.IndexOf(degrees, degrees.Max());
    }
}
