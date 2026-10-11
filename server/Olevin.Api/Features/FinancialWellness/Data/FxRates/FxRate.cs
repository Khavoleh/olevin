using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Data.FxRates;

/// <summary>
/// The exchange rate of a currency to the base currency (USD) on a date, a row of <c>financial_wellness.fx_rates</c>.
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
    /// Amount of base currency (USD) for one unit of currency.
    /// </summary>
    public decimal RateToBase { get; set; }
}
