using Coravel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.NameTranslation;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Jobs;
using Olevin.Api.Features.FinancialWellness.Seeders;
using Olevin.Api.Infrastructure.Database;
using Olevin.Api.Shared;
using Wolverine.EntityFrameworkCore;

namespace Olevin.Api.Features.FinancialWellness;

/// <summary>
/// Registers the Financial Wellness feature.
/// </summary>
public static class FinancialWellnessServices
{
    private const string Schema = "financial_wellness";

    /// <summary>
    /// Keeps the members of the enums as they are, since they match the labels in the database.
    /// </summary>
    private static readonly NpgsqlNullNameTranslator Labels = new();

    /// <summary>
    /// Adds <see cref="FinancialWellnessDbContext"/>, whose changes Wolverine saves in a transaction after each handler,
    /// the seeder that fills missing exchange rates on startup, and the job that updates them daily. The job needs the
    /// scheduler from <c>AddJobs</c>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddFinancialWellness(this IServiceCollection services)
    {
        services.AddDbContextWithWolverineIntegration<FinancialWellnessDbContext>(
            (provider, options) =>
                options.UseNpgsql(
                    provider.GetRequiredService<IOptions<DbOptions>>().Value.Olevin,
                    npgsql => npgsql.MapEnum<CurrencyCode>("currency_code", Schema, Labels)
                )
        );

        services.AddHostedService<FxRateSeeder>();

        services.AddTransient<UpdateFxRatesJob>();

        return services;
    }

    /// <summary>
    /// Schedules the jobs of the Financial Wellness feature.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <returns>The same service provider for chaining.</returns>
    public static IServiceProvider UseFinancialWellnessJobs(this IServiceProvider services)
    {
        services.UseScheduler(scheduler =>
            scheduler
                .Schedule<UpdateFxRatesJob>()
                .Cron("5 * * * *")
                .PreventOverlapping(nameof(UpdateFxRatesJob))
                .RunOnceAtStart()
        );

        return services;
    }
}
