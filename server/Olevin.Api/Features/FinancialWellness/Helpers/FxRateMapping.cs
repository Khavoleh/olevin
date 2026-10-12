using Olevin.Api.Features.FinancialWellness.Data.FxRates;
using Olevin.Api.Infrastructure.ExchangeRate;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Helpers;

/// <summary>
/// Turns the ExchangeRate-API answer into <see cref="FxRate"/> rows.
/// </summary>
public static class FxRateMapping
{
    private const string BaseCode = "USD";
    private const string SuccessResult = "success";

    /// <summary>
    /// Creates a rate row for each requested currency, valid for the date the API updated its rates.
    /// </summary>
    /// <param name="response">The latest rates quoted against the US dollar.</param>
    /// <param name="currencies">The currencies to create rows for.</param>
    /// <returns>The rates against the base currency (USD), one per currency.</returns>
    /// <exception cref="InvalidOperationException">The response is not a successful one in US dollars, or lacks a requested currency.</exception>
    public static List<FxRate> ToFxRates(
        LatestRatesResponse response,
        IEnumerable<CurrencyCode> currencies
    )
    {
        if (response.Result != SuccessResult || response.BaseCode != BaseCode)
        {
            throw new InvalidOperationException(
                $"The exchange rate API returned '{response.Result}' for base '{response.BaseCode}'."
            );
        }

        DateOnly rateDate = DateOnly.FromDateTime(
            DateTimeOffset.FromUnixTimeSeconds(response.TimeLastUpdateUnix).UtcDateTime
        );

        return [.. currencies.Select(currency => ToFxRate(response, currency, rateDate))];
    }

    private static FxRate ToFxRate(
        LatestRatesResponse response,
        CurrencyCode currency,
        DateOnly rateDate
    )
    {
        return new FxRate
        {
            Currency = currency,
            RateDate = rateDate,
            UnitsPerBase =
                response.ConversionRates.TryGetValue(currency.ToString(), out decimal perDollar)
                && perDollar > 0
                    ? perDollar
                    : throw new InvalidOperationException(
                        $"The exchange rate API has no valid rate for {currency}."
                    ),
        };
    }
}
