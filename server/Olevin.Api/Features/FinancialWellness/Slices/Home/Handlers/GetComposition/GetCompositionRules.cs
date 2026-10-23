using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Fuzzy;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
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
    /// Rounds the output of a subsystem.
    /// </summary>
    /// <param name="value">The output from 0 to 100.</param>
    /// <returns>The output rounded to one decimal, halves away from zero.</returns>
    public static double Round(double value) => Math.Round(value, 1, MidpointRounding.AwayFromZero);

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
    /// Gets a share of income, such as SR or DTI.
    /// </summary>
    /// <param name="share">The share before limiting to its range.</param>
    /// <param name="month">The month of the share.</param>
    /// <returns>The share, or <see langword="null"/> without income.</returns>
    public static double? OfIncome(double share, MonthFigures month) =>
        month.Income > 0 ? share : null;

    /// <summary>
    /// Gets the reserve R in months.
    /// </summary>
    /// <param name="inputs">The inputs of the month.</param>
    /// <param name="month">The month.</param>
    /// <returns>R before limiting, or <see langword="null"/> without expenses and debt payments.</returns>
    public static double? ReserveMonths(CalculatedInputs inputs, MonthFigures month) =>
        inputs.AverageExpenses + month.DebtPayments > 0 ? inputs.Raw.Reserve : null;

    /// <summary>
    /// Gets the expense variation CV.
    /// </summary>
    /// <param name="inputs">The inputs of the month.</param>
    /// <returns>CV before limiting, or <see langword="null"/> while the history has one month.</returns>
    public static double? ExpenseVariation(CalculatedInputs inputs) =>
        inputs.HistoryMonths > 1 ? inputs.Raw.ExpenseVariation : null;

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
