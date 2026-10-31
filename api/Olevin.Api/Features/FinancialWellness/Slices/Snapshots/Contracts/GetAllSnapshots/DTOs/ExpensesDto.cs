namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots.DTOs;

/// <summary>
/// The expenses of a month by category as entered, without debt payments.
/// </summary>
/// <param name="Food">Food.</param>
/// <param name="Housing">Housing.</param>
/// <param name="Health">Health and medicines.</param>
/// <param name="Clothing">Clothing.</param>
/// <param name="Restaurants">Restaurants.</param>
/// <param name="Transport">Transport.</param>
/// <param name="Other">Everything else.</param>
public sealed record ExpensesDto(
    MoneyDto Food,
    MoneyDto Housing,
    MoneyDto Health,
    MoneyDto Clothing,
    MoneyDto Restaurants,
    MoneyDto Transport,
    MoneyDto Other
);
