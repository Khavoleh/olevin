using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Data.FxRates;

/// <summary>
/// The exchange rate of a currency against the base currency (USD) on a date, a row of <c>financial_wellness.fx_rates</c>.
/// </summary>
public sealed class FxRate
{
    /// <summary>
    /// Currency the rate is quoted for.
    /// </summary>
    public CurrencyCode Currency { get; set; }

    /// <summary>
    /// Date the rate is valid for.
    /// </summary>
    public DateOnly RateDate { get; set; }

    /// <summary>
    /// Amount of the currency for one unit of the base currency (USD), as the exchange rate API quotes it.
    /// </summary>
    public decimal UnitsPerBase { get; set; }
}
