using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Helpers.History;

/// <summary>
/// Exchange rates on one date, quoted against the US dollar.
/// </summary>
/// <param name="unitsPerUsd">The number of units of every currency per one US dollar.</param>
public sealed class ExchangeRates(IReadOnlyDictionary<CurrencyCode, decimal> unitsPerUsd)
{
    private readonly Dictionary<CurrencyCode, decimal> UnitsPerUsd = new(unitsPerUsd)
    {
        [CurrencyCode.USD] = 1,
    };

    /// <summary>
    /// Converts an amount to another currency.
    /// </summary>
    /// <param name="money">The amount.</param>
    /// <param name="target">The currency to convert to.</param>
    /// <returns>The amount in <paramref name="target"/>.</returns>
    /// <exception cref="InvalidOperationException">There is no rate for one of the currencies.</exception>
    public decimal Convert(Money money, CurrencyCode target)
    {
        return money.Currency == target
            ? money.Amount
            : money.Amount * UnitsOf(target) / UnitsOf(money.Currency);
    }

    private decimal UnitsOf(CurrencyCode currency)
    {
        return UnitsPerUsd.TryGetValue(currency, out decimal units)
            ? units
            : throw new InvalidOperationException($"There is no rate for {currency}.");
    }
}
