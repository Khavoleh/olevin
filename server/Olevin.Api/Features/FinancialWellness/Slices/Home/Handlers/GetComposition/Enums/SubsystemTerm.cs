namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetComposition.Enums;

/// <summary>
/// The term of the "cash flow" or "financial buffer" subsystem with the highest degree of membership.
/// </summary>
public enum SubsystemTerm
{
    /// <summary>
    /// Around 0.
    /// </summary>
    Weak,

    /// <summary>
    /// Around 50.
    /// </summary>
    Moderate,

    /// <summary>
    /// Around 100.
    /// </summary>
    Strong,
}
