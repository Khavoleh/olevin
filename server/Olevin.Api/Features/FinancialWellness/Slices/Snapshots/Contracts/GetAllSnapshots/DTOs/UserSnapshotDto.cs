namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots.DTOs;

/// <summary>
/// A copy of a stored snapshot of the user: every column of the row except the owner and the concurrency token.
/// </summary>
/// <param name="Id">The snapshot identifier.</param>
/// <param name="Month">The month of the snapshot, as the name stored in the database.</param>
/// <param name="Year">The year of the snapshot.</param>
/// <param name="FxDate">The date of the exchange rates the amounts are converted at.</param>
/// <param name="SelfRating">The self-rating of calm from 1 to 5.</param>
/// <param name="SavingsRate">The savings rate (SR).</param>
/// <param name="ReserveMonths">The liquid reserves in months of spending (R).</param>
/// <param name="DebtToIncome">The debt payments to income (DTI).</param>
/// <param name="ExpenseVariation">The coefficient of variation of expenses (CV).</param>
/// <param name="SavingsRegularity">The share of recent months with savings (RG).</param>
/// <param name="CashFlow">The output of the cash flow subsystem (0-100).</param>
/// <param name="Buffer">The output of the financial buffer subsystem (0-100).</param>
/// <param name="FuzzyIndex">The fuzzy index (0-100).</param>
/// <param name="LinearIndex">The linear index for comparison (0-100).</param>
/// <param name="FinalIndex">The final index corrected by the self-rating (0-100).</param>
/// <param name="Income">The net income, possibly in several currencies.</param>
/// <param name="Expenses">The expenses by category, without debt payments.</param>
/// <param name="Saved">The amount saved during the month.</param>
/// <param name="Reserves">The liquid reserves at the end of the month, possibly in several currencies.</param>
/// <param name="DebtPayments">The monthly debt payments, possibly in several currencies.</param>
/// <param name="ModelVersion">The version of the index model the results were calculated with.</param>
/// <param name="CreatedAt">When the snapshot was created.</param>
/// <param name="UpdatedAt">When the snapshot was last changed.</param>
public sealed record UserSnapshotDto(
    Guid Id,
    string Month,
    short Year,
    DateOnly FxDate,
    short SelfRating,
    double SavingsRate,
    double ReserveMonths,
    double DebtToIncome,
    double ExpenseVariation,
    double SavingsRegularity,
    double CashFlow,
    double Buffer,
    double FuzzyIndex,
    double LinearIndex,
    double FinalIndex,
    IReadOnlyList<MoneyDto> Income,
    ExpensesDto Expenses,
    MoneyDto Saved,
    IReadOnlyList<MoneyDto> Reserves,
    IReadOnlyList<MoneyDto> DebtPayments,
    string ModelVersion,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
