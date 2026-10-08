namespace Olevin.Api.Shared.Index.Indicators;

/// <summary>
/// The inputs of the model. The order is also the order of advice with equal gains.
/// </summary>
public enum Indicator
{
    /// <summary>
    /// Savings rate SR: the share of income left after expenses and debt payments.
    /// </summary>
    SavingsRate,

    /// <summary>
    /// Reserve R: how many months of expenses and debt payments the liquid reserves cover.
    /// </summary>
    Reserve,

    /// <summary>
    /// Debt-to-income DTI: the share of income spent on debt payments.
    /// </summary>
    DebtToIncome,

    /// <summary>
    /// Expense variation CV: the coefficient of variation of monthly expenses.
    /// </summary>
    ExpenseVariation,

    /// <summary>
    /// Savings regularity RG: the share of recent months with something saved.
    /// </summary>
    SavingsRegularity,
}
