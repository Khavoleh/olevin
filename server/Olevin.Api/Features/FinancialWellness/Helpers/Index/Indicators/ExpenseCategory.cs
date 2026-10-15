namespace Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;

/// <summary>
/// A category of monthly expenses.
/// </summary>
public enum ExpenseCategory
{
    /// <summary>
    /// Food; partly flexible.
    /// </summary>
    Food,

    /// <summary>
    /// Housing; required.
    /// </summary>
    Housing,

    /// <summary>
    /// Health and medicines; required.
    /// </summary>
    Health,

    /// <summary>
    /// Clothing; flexible.
    /// </summary>
    Clothing,

    /// <summary>
    /// Restaurants; flexible.
    /// </summary>
    Restaurants,

    /// <summary>
    /// Transport; required.
    /// </summary>
    Transport,

    /// <summary>
    /// Everything else; flexible.
    /// </summary>
    Other,
}
