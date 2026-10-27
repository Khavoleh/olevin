using Coravel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.NameTranslation;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.PushSubscriptions;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Features.Settings.Slices.SnapshotDayPush;
using Olevin.Api.Infrastructure.Database;
using Olevin.Api.Shared;
using Wolverine.EntityFrameworkCore;

namespace Olevin.Api.Features.Settings;

/// <summary>
/// Registers the Settings feature.
/// </summary>
public static class SettingsServices
{
    private const string Schema = "settings";
    private const string FinancialWellnessSchema = "financial_wellness";

    /// <summary>
    /// Keeps the members of the enums as they are, since they match the labels in the database.
    /// </summary>
    private static readonly NpgsqlNullNameTranslator Labels = new();

    /// <summary>
    /// Adds <see cref="SettingsDbContext"/>, whose changes Wolverine saves in a transaction after each handler, and the
    /// job that queues the push on the snapshot day. The job needs the scheduler from <c>AddJobs</c> and the Web Push
    /// client from <c>AddWebPush</c>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddDbContextWithWolverineIntegration<SettingsDbContext>(
            (provider, options) =>
                options.UseNpgsql(
                    provider.GetRequiredService<IOptions<DbOptions>>().Value.Settings,
                    npgsql =>
                    {
                        npgsql.MapEnum<CurrencyCode>(
                            "currency_code",
                            FinancialWellnessSchema,
                            Labels
                        );
                        npgsql.MapEnum<CalendarMonth>(
                            "calendar_month",
                            FinancialWellnessSchema,
                            Labels
                        );
                        npgsql.MapEnum<LocaleCode>("locale_code", Schema, Labels);
                    }
                ),
            MessagePersistence.Schema
        );

        services.AddTransient<SendSnapshotDayPushJob>();

        return services;
    }

    /// <summary>
    /// Schedules the jobs of the Settings feature.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <returns>The same service provider for chaining.</returns>
    public static IServiceProvider UseSettingsJobs(this IServiceProvider services)
    {
        services.UseScheduler(scheduler =>
            scheduler
                .Schedule<SendSnapshotDayPushJob>()
                .EveryFiveMinutes()
                .PreventOverlapping(nameof(SendSnapshotDayPushJob))
        );

        return services;
    }
}
