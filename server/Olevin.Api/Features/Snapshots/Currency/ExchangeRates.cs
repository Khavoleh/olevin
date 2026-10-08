namespace Olevin.Api.Features.Snapshots.Currency;

/// <summary>
/// Official NBU rates to the hryvnia on one date. Rates between other currencies are crossed through the hryvnia:
/// <c>A_base = A_entered · K_entered / K_base</c>.
/// </summary>
/// <param name="rates">The number of hryvnias per unit of every other currency; the hryvnia itself is 1.</param>
public sealed class ExchangeRates(IReadOnlyDictionary<CurrencyCode, decimal> rates)
{
    private readonly Dictionary<CurrencyCode, decimal> Rates = new(rates)
    {
        [CurrencyCode.UAH] = 1,
    };

    /// <summary>
    /// Gets a value indicating whether there is a rate for a currency.
    /// </summary>
    /// <param name="currency">The currency.</param>
    /// <returns><see langword="true"/> for the hryvnia and for every currency with a rate.</returns>
    public bool Has(CurrencyCode currency) => Rates.ContainsKey(currency);

    /// <summary>
    /// Converts an amount to another currency.
    /// </summary>
    /// <param name="money">The amount.</param>
    /// <param name="target">The currency to convert to.</param>
    /// <returns>The amount in <paramref name="target"/>.</returns>
    public decimal Convert(Money money, CurrencyCode target)
    {
        return money.Currency == target
            ? money.Amount
            : money.Amount * RateToUah(money.Currency) / RateToUah(target);
    }

    private decimal RateToUah(CurrencyCode currency)
    {
        return Rates.TryGetValue(currency, out decimal rate)
            ? rate
            : throw new InvalidOperationException($"There is no rate for {currency}.");
    }
}
