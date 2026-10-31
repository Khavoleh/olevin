namespace Olevin.Api.Features.FinancialWellness.Slices.Trend.Handlers;

/// <summary>
/// The months the trend covers.
/// </summary>
public enum TrendPeriod
{
    /// <summary>
    /// The last 6 months.
    /// </summary>
    SixMonths,

    /// <summary>
    /// The last 12 months.
    /// </summary>
    TwelveMonths,

    /// <summary>
    /// Every month since the first snapshot.
    /// </summary>
    All,
}
