using System.Text.Json.Serialization;

namespace Olevin.Api.Infrastructure.ExchangeRate;

/// <summary>
/// The response of the ExchangeRate-API <c>latest</c> endpoint.
/// </summary>
/// <param name="Result">Whether the request succeeded: <c>success</c> or <c>error</c>.</param>
/// <param name="TimeLastUpdateUnix">The time the rates were last updated, in Unix seconds (UTC).</param>
/// <param name="BaseCode">The currency the rates are quoted against.</param>
/// <param name="ConversionRates">The amount of each currency for one unit of the base currency, by currency code.</param>
public sealed record LatestRatesResponse(
    [property: JsonPropertyName("result")] string Result,
    [property: JsonPropertyName("time_last_update_unix")] long TimeLastUpdateUnix,
    [property: JsonPropertyName("base_code")] string BaseCode,
    [property: JsonPropertyName("conversion_rates")]
        IReadOnlyDictionary<string, decimal> ConversionRates
);
