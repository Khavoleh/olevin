using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Features.Advice;

/// <summary>
/// The parameters of the advice.
/// </summary>
public static class AdviceConstants
{
    /// <summary>
    /// How many advice the user sees.
    /// </summary>
    public const int ShownAdvice = 3;

    /// <summary>
    /// The flexible share of food expenses, λ.
    /// </summary>
    public const double FoodFlexibleShare = 0.1;

    /// <summary>
    /// The step of SR, DTI and CV: 5 percentage points.
    /// </summary>
    public const double RateStep = 0.05;

    /// <summary>
    /// The number of months over which the reserve is topped up from the surplus.
    /// </summary>
    public const int ReserveTopUpMonths = 3;

    /// <summary>
    /// The largest step of R, in months.
    /// </summary>
    public const double MaxReserveStep = 1;

    /// <summary>
    /// Gains closer than this are equal; the order of <see cref="Indicator"/> then decides.
    /// </summary>
    public const double Tolerance = 1e-6;
}
