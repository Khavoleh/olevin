namespace Olevin.Api.Infrastructure.Database;

/// <summary>
/// Registers the database settings.
/// </summary>
public static class DatabaseServices
{
    /// <summary>
    /// Fills <see cref="DbOptions"/> from the <c>ConnectionStrings</c> section and checks it when the application starts.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddOptions<DbOptions>()
            .Bind(configuration.GetRequiredSection(DbOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Olevin),
                "The Olevin connection string is missing."
            )
            .ValidateOnStart();

        return services;
    }
}
