namespace Olevin.Api.Infrastructure.Telemetry;

/// <summary>
/// Settings of the Seq server that receives logs, traces and metrics.
/// </summary>
public sealed class SeqOptions
{
    /// <summary>
    /// The configuration section that holds these settings.
    /// </summary>
    public const string SectionName = "Seq";

    /// <summary>
    /// Gets the base URL of the Seq server.
    /// </summary>
    public required Uri ServerUrl { get; init; }

    /// <summary>
    /// Gets the Seq API key.
    /// </summary>
    public required string ApiKey { get; init; }
}
