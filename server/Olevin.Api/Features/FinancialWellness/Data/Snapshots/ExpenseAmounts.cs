namespace Olevin.Api.Features.FinancialWellness.Data.Snapshots;

/// <summary>
/// The expenses of the month by category as entered, the <c>expenses</c> part of <c>snapshots.amounts</c>.
/// A category that was not entered is <see cref="Money.Zero"/> in the base currency.
/// </summary>
/// <param name="Food">Food.</param>
/// <param name="Housing">Housing.</param>
/// <param name="Health">Health and medicines.</param>
/// <param name="Clothing">Clothing.</param>
/// <param name="Restaurants">Restaurants.</param>
/// <param name="Transport">Transport.</param>
/// <param name="Other">Everything else.</param>
public sealed record ExpenseAmounts(
    Money Food,
    Money Housing,
    Money Health,
    Money Clothing,
    Money Restaurants,
    Money Transport,
    Money Other
);
