using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Data.FxRates;

/// <summary>
/// Daily exchange rates of currencies against the base currency (USD). A row of <c>financial_wellness.fx_rates</c>.
/// </summary>
public sealed class FxRate
{
    /// <summary>
    /// Currency the rate is quoted for.
    /// </summary>
    public CurrencyCode Currency { get; set; }

    /// <summary>
    /// Amount of the currency for one unit of the base currency (USD).
    /// </summary>
    public decimal UnitsPerBase { get; set; }

    /// <summary>
    /// Date the rate is valid for.
    /// </summary>
    public DateOnly RateDate { get; set; }
}
