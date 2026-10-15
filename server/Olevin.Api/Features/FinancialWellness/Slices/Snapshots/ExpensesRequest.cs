using Olevin.Api.Features.FinancialWellness.Data.Snapshots;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots;

/// <summary>
/// The expenses of a month by category.
/// </summary>
/// <param name="Food">Food.</param>
/// <param name="Housing">Housing.</param>
/// <param name="Health">Health and medicines.</param>
/// <param name="Clothing">Clothing.</param>
/// <param name="Restaurants">Restaurants.</param>
/// <param name="Transport">Transport.</param>
/// <param name="Other">Everything else.</param>
public sealed record ExpensesRequest(
    Money Food,
    Money Housing,
    Money Health,
    Money Clothing,
    Money Restaurants,
    Money Transport,
    Money Other
);
