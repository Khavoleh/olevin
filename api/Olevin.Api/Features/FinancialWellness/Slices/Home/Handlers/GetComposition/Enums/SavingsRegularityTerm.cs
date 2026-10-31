namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Enums;

/// <summary>
/// The term of the savings regularity RG with the highest degree of membership.
/// </summary>
public enum SavingsRegularityTerm
{
    /// <summary>
    /// Up to about a third of the months with savings.
    /// </summary>
    Rarely,

    /// <summary>
    /// About half of the months with savings.
    /// </summary>
    Sometimes,

    /// <summary>
    /// Most of the months with savings.
    /// </summary>
    Regularly,
}
