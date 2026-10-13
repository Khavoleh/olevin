using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Data.Snapshots;

/// <summary>
/// Monthly snapshots of a user's finances: the entered amounts and the index calculated from them. A row of
/// <c>financial_wellness.snapshots</c>.
/// </summary>
public sealed class Snapshot
{
    /// <summary>
    /// Snapshot identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Owning user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Month of the snapshot.
    /// </summary>
    public CalendarMonth Month { get; set; }

    /// <summary>
    /// Year of the snapshot.
    /// </summary>
    public short Year { get; set; }

    /// <summary>
    /// Date of the exchange rates the amounts are converted at.
    /// </summary>
    public DateOnly FxDate { get; set; }

    /// <summary>
    /// Currency the results were calculated in.
    /// </summary>
    public CurrencyCode BaseCurrency { get; set; }

    /// <summary>
    /// Self-rated financial calm (1-5).
    /// </summary>
    public short SelfRating { get; set; }

    /// <summary>
    /// Savings rate (SR), clamped to its range.
    /// </summary>
    public double SavingsRate { get; set; }

    /// <summary>
    /// Liquid reserves in months of spending (R), clamped to its range.
    /// </summary>
    public double ReserveMonths { get; set; }

    /// <summary>
    /// Debt payments to income (DTI), clamped to its range.
    /// </summary>
    public double DebtToIncome { get; set; }

    /// <summary>
    /// Coefficient of variation of expenses (CV), clamped to its range.
    /// </summary>
    public double ExpenseVariation { get; set; }

    /// <summary>
    /// Share of recent months with savings (RG).
    /// </summary>
    public double SavingsRegularity { get; set; }

    /// <summary>
    /// Output of the cash flow subsystem (0-100).
    /// </summary>
    public double CashFlow { get; set; }

    /// <summary>
    /// Output of the financial buffer subsystem (0-100).
    /// </summary>
    public double Buffer { get; set; }

    /// <summary>
    /// Fuzzy index (0-100).
    /// </summary>
    public double FuzzyIndex { get; set; }

    /// <summary>
    /// Linear index for comparison, not shown to the user (0-100).
    /// </summary>
    public double LinearIndex { get; set; }

    /// <summary>
    /// Final index corrected by the self-rating (0-100).
    /// </summary>
    public double FinalIndex { get; set; }

    /// <summary>
    /// Entered amounts, each in the currency it was entered in.
    /// </summary>
    public SnapshotAmounts Amounts { get; set; } = null!;

    /// <summary>
    /// Version of the index model the results were calculated with.
    /// </summary>
    public string ModelVersion { get; set; } = null!;

    /// <summary>
    /// When the snapshot was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the snapshot was last changed.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Concurrency token (the system column <c>xmin</c>).
    /// </summary>
    public uint Version { get; set; }
}
