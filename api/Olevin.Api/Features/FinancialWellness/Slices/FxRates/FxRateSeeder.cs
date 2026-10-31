using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Infrastructure.ExchangeRate;
using Olevin.Api.Infrastructure.Telemetry;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.FxRates;

/// <summary>
/// When the application starts, makes sure every supported currency has an exchange rate in the database, taking
/// missing ones from the API. A failure stops the application from starting.
/// </summary>
/// <param name="scopeFactory">The factory of the scope the seeding runs in.</param>
/// <param name="options">The ExchangeRate-API settings.</param>
/// <param name="logger">The logger.</param>
public sealed class FxRateSeeder(
    IServiceScopeFactory scopeFactory,
    IOptions<ExchangeRateOptions> options,
    ILogger<FxRateSeeder> logger
) : IHostedService
{
    private static readonly ActivitySource Source = new(Telemetry.ActivitySourceName);

    /// <summary>
    /// Adds a rate for every currency that has none yet. Does nothing when all currencies have rates.
    /// </summary>
    /// <param name="cancellationToken">The token to cancel the seeding.</param>
    /// <returns>A task that completes when the rates are saved.</returns>
    /// <exception cref="Refit.ApiException">The exchange rate API answered with an error.</exception>
    /// <exception cref="Refit.ApiRequestException">The exchange rate API is unreachable.</exception>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using Activity? activity = Source.StartActivity("Seed exchange rates");

        try
        {
            await SeedAsync(activity, cancellationToken);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
    }

    private async Task SeedAsync(Activity? activity, CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        FinancialWellnessDbContext db =
            scope.ServiceProvider.GetRequiredService<FinancialWellnessDbContext>();
        IExchangeRateApi api = scope.ServiceProvider.GetRequiredService<IExchangeRateApi>();

        List<CurrencyCode> existing = await db
            .FxRates.Select(rate => rate.Currency)
            .Distinct()
            .ToListAsync(cancellationToken);
        List<CurrencyCode> missing = [.. Enum.GetValues<CurrencyCode>().Except(existing)];

        activity?.SetTag("fx.missing_currencies", string.Join(",", missing));

        if (missing.Count == 0)
            return;

        logger.LogInformation(
            "Seeding exchange rates of {Currencies}.",
            string.Join(", ", missing)
        );

        LatestRatesResponse response = await api.GetLatestUsdRatesAsync(
            options.Value.ApiKey,
            cancellationToken
        );

        db.FxRates.AddRange(FxRateMapping.ToFxRates(response, missing));
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
