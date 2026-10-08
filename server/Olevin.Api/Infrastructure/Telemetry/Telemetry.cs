using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Olevin.Api.Infrastructure.Telemetry;

/// <summary>
/// Logs through Serilog, traces and metrics through OpenTelemetry; everything goes to Seq.
/// </summary>
public static class Telemetry
{
    /// <summary>
    /// Adds Serilog logging and OpenTelemetry tracing and metrics exported to Seq.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The same builder for chaining.</returns>
    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder)
    {
        SeqOptions seq =
            builder.Configuration.GetRequiredSection(SeqOptions.SectionName).Get<SeqOptions>()
            ?? throw new InvalidOperationException("Seq settings are missing.");

        builder.Services.AddSerilog(
            (services, logger) =>
                logger
                    .ReadFrom.Configuration(builder.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .WriteTo.Console()
                    .WriteTo.Seq(seq.ServerUrl.ToString(), apiKey: seq.ApiKey)
        );

        builder
            .Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("Wolverine")
                    .AddOtlpExporter(exporter =>
                        ExportToSeq(exporter, seq, "ingest/otlp/v1/traces")
                    )
            )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Wolverine")
                    .AddOtlpExporter(exporter =>
                        ExportToSeq(exporter, seq, "ingest/otlp/v1/metrics")
                    )
            );

        return builder;
    }

    private static void ExportToSeq(OtlpExporterOptions exporter, SeqOptions seq, string path)
    {
        exporter.Endpoint = new Uri(seq.ServerUrl, path);
        exporter.Protocol = OtlpExportProtocol.HttpProtobuf;

        if (!string.IsNullOrEmpty(seq.ApiKey))
            exporter.Headers = $"X-Seq-ApiKey={seq.ApiKey}";
    }
}
