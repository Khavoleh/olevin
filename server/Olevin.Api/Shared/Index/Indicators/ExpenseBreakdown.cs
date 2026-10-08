namespace Olevin.Api.Shared.Index.Indicators;

/// <summary>
/// Monthly expenses by category in the base currency.
/// </summary>
/// <param name="Food">Food.</param>
/// <param name="Housing">Housing.</param>
/// <param name="Health">Health and medicines.</param>
/// <param name="Clothing">Clothing.</param>
/// <param name="Restaurants">Restaurants.</param>
/// <param name="Transport">Transport.</param>
/// <param name="Other">Everything else.</param>
public sealed record ExpenseBreakdown(
    double Food,
    double Housing,
    double Health,
    double Clothing,
    double Restaurants,
    double Transport,
    double Other
)
{
    /// <summary>
    /// Gets the sum of all categories.
    /// </summary>
    public double Total => Food + Housing + Health + Clothing + Restaurants + Transport + Other;

    /// <summary>
    /// Gets the parts of the expenses that a person can cut: restaurants, clothing, other and a share of food.
    /// </summary>
    /// <param name="foodShare">The flexible share of food, λ.</param>
    /// <returns>The flexible amount of every category that has one.</returns>
    public IReadOnlyList<(ExpenseCategory Category, double Amount)> FlexibleParts(double foodShare)
    {
        return
        [
            (ExpenseCategory.Other, Other),
            (ExpenseCategory.Restaurants, Restaurants),
            (ExpenseCategory.Clothing, Clothing),
            (ExpenseCategory.Food, foodShare * Food),
        ];
    }
}
