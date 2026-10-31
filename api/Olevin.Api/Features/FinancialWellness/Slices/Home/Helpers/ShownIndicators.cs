using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Helpers;

/// <summary>
/// Gives the indicators as the user sees them: before limiting, and empty where they mean nothing.
/// </summary>
public static class ShownIndicators
{
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
}
