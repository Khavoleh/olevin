using Microsoft.Extensions.Options;
using Refit;

namespace Olevin.Api.Infrastructure.ExchangeRate;

/// <summary>
/// Registers the ExchangeRate-API client.
/// </summary>
public static class ExchangeRateServices
{
    /// <summary>
    /// Fills <see cref="ExchangeRateOptions"/> from the <c>ExchangeRate</c> section, checks it when the application
    /// starts, and adds <see cref="IExchangeRateApi"/> with the standard resilience pipeline (retry, circuit breaker, timeouts).
    /// The client does not log its requests, because the API key is a part of the URL.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddExchangeRateApi(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddOptions<ExchangeRateOptions>()
            .Bind(configuration.GetRequiredSection(ExchangeRateOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ApiKey),
                "The ExchangeRate API key is missing."
            )
            .ValidateOnStart();

        services
            .AddRefitGeneratedClient<IExchangeRateApi>()
            .ConfigureHttpClient(
                (provider, client) =>
                    client.BaseAddress = provider
                        .GetRequiredService<IOptions<ExchangeRateOptions>>()
                        .Value.BaseUrl
            )
            .RemoveAllLoggers()
            .AddStandardResilienceHandler();

        return services;
    }
}
