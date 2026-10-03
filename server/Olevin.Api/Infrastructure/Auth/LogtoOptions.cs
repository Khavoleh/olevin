namespace Olevin.Api.Infrastructure.Auth;

/// <summary>
/// Settings of the Logto tenant that issues access tokens for this API.
/// </summary>
public sealed class LogtoOptions
{
    /// <summary>
    /// The configuration section that holds these settings.
    /// </summary>
    public const string SectionName = "Logto";

    /// <summary>
    /// Gets the public Logto endpoint.
    /// </summary>
    public required Uri Endpoint { get; init; }

    /// <summary>
    /// Gets the API resource indicator registered in Logto.
    /// </summary>
    public required string ApiResource { get; init; }
}
