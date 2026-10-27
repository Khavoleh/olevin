using System.Diagnostics;
using Npgsql;
using Olevin.Api.Infrastructure.ExchangeRate;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace Olevin.Api.Infrastructure.Telemetry;

/// <summary>
/// Logs through Serilog, traces and metrics (including database calls) through OpenTelemetry; everything goes over
/// OTLP to the OpenTelemetry Collector. Errors also go to GlitchTip through the Sentry SDK.
/// </summary>
public static class Telemetry
{
    /// <summary>
    /// The name of the activity source for the spans the application creates itself, such as the startup seeding.
    /// </summary>
    public const string ActivitySourceName = "Olevin.Api";

    /// <summary>
    /// Adds Serilog logging and OpenTelemetry tracing and metrics exported to the OpenTelemetry Collector, and error
    /// reporting to GlitchTip.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The same builder for chaining.</returns>
    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder)
    {
        TelemetryOptions telemetry =
            builder
                .Configuration.GetRequiredSection(TelemetryOptions.SectionName)
                .Get<TelemetryOptions>()
            ?? throw new InvalidOperationException("Telemetry settings are missing.");

        Dictionary<string, object> resource = new()
        {
            ["service.name"] = builder.Environment.ApplicationName,
            ["deployment.environment.name"] = builder.Environment.EnvironmentName,
        };

        builder.Services.AddSerilog(
            (services, logger) =>
                logger
                    .ReadFrom.Configuration(builder.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.OpenTelemetry(exporter =>
                    {
                        exporter.Endpoint = new Uri(telemetry.OtlpEndpoint, "v1/logs").ToString();
                        exporter.Protocol = OtlpProtocol.HttpProtobuf;
                        exporter.ResourceAttributes = resource;
                    })
                    .WriteTo.Sentry(
                        minimumEventLevel: LogEventLevel.Error,
                        minimumBreadcrumbLevel: LogEventLevel.Information
                    )
        );

        builder.WebHost.UseSentry(options => options.SetBeforeSend(AddTraceId));

        builder
            .Services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => resourceBuilder.AddAttributes(resource))
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation(options =>
                        options.EnrichWithHttpRequestMessage = (activity, _) =>
                            ExchangeRateRedaction.RedactSpan(activity)
                    )
                    .AddNpgsql()
                    .AddSource(ActivitySourceName)
                    .AddSource("Wolverine")
                    .AddOtlpExporter(exporter => ExportTo(exporter, telemetry, "v1/traces"))
            )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Npgsql")
                    .AddMeter("Wolverine")
                    .AddOtlpExporter(exporter => ExportTo(exporter, telemetry, "v1/metrics"))
            );

        return builder;
    }

    private static SentryEvent AddTraceId(SentryEvent sentryEvent, SentryHint hint)
    {
        if (Activity.Current is { } activity)
            sentryEvent.SetTag("trace_id", activity.TraceId.ToHexString());

        return sentryEvent;
    }

    private static void ExportTo(
        OtlpExporterOptions exporter,
        TelemetryOptions telemetry,
        string path
    )
    {
        exporter.Endpoint = new Uri(telemetry.OtlpEndpoint, path);
        exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
    }
}
