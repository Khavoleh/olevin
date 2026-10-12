namespace Olevin.Api.Infrastructure.ExchangeRate;

/// <summary>
/// Settings of the ExchangeRate-API service that provides currency exchange rates.
/// </summary>
public sealed class ExchangeRateOptions
{
    /// <summary>
    /// The configuration section that holds these settings.
    /// </summary>
    public const string SectionName = "ExchangeRate";

    /// <summary>
    /// Gets the base URL of the ExchangeRate-API service.
    /// </summary>
    public required Uri BaseUrl { get; init; }

    /// <summary>
    /// Gets the ExchangeRate-API key.
    /// </summary>
    public required string ApiKey { get; init; }
}
