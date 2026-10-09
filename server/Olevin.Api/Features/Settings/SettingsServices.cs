using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.NameTranslation;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.Users.Enums;
using Olevin.Api.Infrastructure.Database;
using Wolverine.EntityFrameworkCore;

namespace Olevin.Api.Features.Settings;

/// <summary>
/// Registers the Settings feature.
/// </summary>
public static class SettingsServices
{
    private const string Schema = "settings";

    /// <summary>
    /// Keeps the members of the enums as they are, since they match the labels in the database.
    /// </summary>
    private static readonly NpgsqlNullNameTranslator Labels = new();

    /// <summary>
    /// Adds <see cref="SettingsDbContext"/>, whose changes Wolverine saves in a transaction after each handler.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddDbContextWithWolverineIntegration<SettingsDbContext>(
            (provider, options) =>
                options.UseNpgsql(
                    provider.GetRequiredService<IOptions<DbOptions>>().Value.Olevin,
                    npgsql =>
                    {
                        npgsql.MapEnum<CurrencyCode>("currency_code", Schema, Labels);
                        npgsql.MapEnum<LocaleCode>("locale_code", Schema, Labels);
                    }
                )
        );

        return services;
    }
}
