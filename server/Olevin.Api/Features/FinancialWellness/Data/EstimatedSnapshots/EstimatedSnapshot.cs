using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;

/// <summary>
/// Rough months entered at onboarding before the first snapshot; never removed. A row of
/// <c>financial_wellness.estimated_snapshots</c>.
/// </summary>
public sealed class EstimatedSnapshot
{
    /// <summary>
    /// Estimated snapshot identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Owning user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Month of the estimated snapshot.
    /// </summary>
    public CalendarMonth Month { get; set; }

    /// <summary>
    /// Year of the estimated snapshot.
    /// </summary>
    public short Year { get; set; }

    /// <summary>
    /// Date of the exchange rates the amounts are converted at.
    /// </summary>
    public DateOnly FxDate { get; set; }

    /// <summary>
    /// Currency the total expenses were entered in.
    /// </summary>
    public CurrencyCode ExpensesCurrency { get; set; }

    /// <summary>
    /// Total expenses of the month.
    /// </summary>
    public decimal ExpensesAmount { get; set; }

    /// <summary>
    /// Currency the saved amount was entered in.
    /// </summary>
    public CurrencyCode SavedCurrency { get; set; }

    /// <summary>
    /// Amount saved in the month.
    /// </summary>
    public decimal SavedAmount { get; set; }

    /// <summary>
    /// When the estimated snapshot was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the estimated snapshot was last changed.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Concurrency token (the system column <c>xmin</c>).
    /// </summary>
    public uint Version { get; set; }
}
