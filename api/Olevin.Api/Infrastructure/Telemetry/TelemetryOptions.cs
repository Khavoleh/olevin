namespace Olevin.Api.Infrastructure.Telemetry;

/// <summary>
/// Settings of the OpenTelemetry Collector that receives logs, traces and metrics.
/// </summary>
public sealed class TelemetryOptions
{
    /// <summary>
    /// The configuration section that holds these settings.
    /// </summary>
    public const string SectionName = "Telemetry";

    /// <summary>
    /// Gets the base URL of the collector's OTLP/HTTP receiver.
    /// </summary>
    public required Uri OtlpEndpoint { get; init; }
}
