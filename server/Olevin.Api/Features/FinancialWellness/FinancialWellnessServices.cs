using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.NameTranslation;
using Olevin.Api.Features.FinancialWellness.Data;
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
    /// and the seeder that fills missing exchange rates on startup.
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

        return services;
    }
}
