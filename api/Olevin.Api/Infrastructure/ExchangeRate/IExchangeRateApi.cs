using Refit;

namespace Olevin.Api.Infrastructure.ExchangeRate;

/// <summary>
/// The ExchangeRate-API service.
/// </summary>
public interface IExchangeRateApi
{
    /// <summary>
    /// Gets the latest exchange rates of all currencies against the US dollar.
    /// </summary>
    /// <param name="apiKey">The ExchangeRate-API key.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The latest rates quoted for one US dollar.</returns>
    [Get("/v6/{apiKey}/latest/USD")]
    Task<LatestRatesResponse> GetLatestUsdRatesAsync(
        string apiKey,
        CancellationToken cancellationToken
    );
}
