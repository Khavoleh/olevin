using System.Diagnostics;
using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.FxRates;
using Olevin.Api.Infrastructure.ExchangeRate;
using Olevin.Api.Infrastructure.Telemetry;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.FxRates;

/// <summary>
/// Stores the daily exchange rates the API publishes. The job runs every hour and calls the API only while the rates of
/// today (UTC) are missing.
/// </summary>
/// <param name="db">The database of the Financial Wellness feature.</param>
/// <param name="api">The ExchangeRate-API client.</param>
/// <param name="options">The ExchangeRate-API settings.</param>
/// <param name="time">The clock that tells what day it is.</param>
/// <param name="logger">The logger.</param>
public sealed class UpdateFxRatesJob(
    FinancialWellnessDbContext db,
    IExchangeRateApi api,
    IOptions<ExchangeRateOptions> options,
    TimeProvider time,
    ILogger<UpdateFxRatesJob> logger
) : IInvocable, ICancellableInvocable
{
    private static readonly ActivitySource Source = new(Telemetry.ActivitySourceName);

    /// <summary>
    /// Gets or sets the token that is cancelled when the application shuts down. Set by Coravel.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Stores the rates of today when they are missing and the API has published them.
    /// </summary>
    /// <returns>A task that completes when the run ends.</returns>
    public async Task Invoke()
    {
        using Activity? activity = Source.StartActivity("Update exchange rates");

        try
        {
            await UpdateAsync();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            logger.LogError(exception, "Updating exchange rates failed.");
        }
    }

    private async Task UpdateAsync()
    {
        DateOnly today = DateOnly.FromDateTime(time.GetUtcNow().UtcDateTime);

        List<CurrencyCode> missing = await MissingOnAsync(today);

        if (missing.Count == 0)
            return;

        LatestRatesResponse response = await api.GetLatestUsdRatesAsync(
            options.Value.ApiKey,
            CancellationToken
        );

        List<FxRate> fetched = FxRateMapping.ToFxRates(response, Enum.GetValues<CurrencyCode>());
        DateOnly rateDate = fetched[0].RateDate;

        if (rateDate != today)
            missing = await MissingOnAsync(rateDate);

        List<FxRate> added = [.. fetched.Where(rate => missing.Contains(rate.Currency))];

        if (added.Count == 0)
        {
            logger.LogInformation(
                "The API has not published the rates of {Today} yet, its newest are of {RateDate}.",
                today,
                rateDate
            );
            return;
        }

        db.FxRates.AddRange(added);
        await db.SaveChangesAsync(CancellationToken);

        logger.LogInformation("Stored the exchange rates of {RateDate}.", rateDate);
    }

    private async Task<List<CurrencyCode>> MissingOnAsync(DateOnly date)
    {
        List<CurrencyCode> stored = await db
            .FxRates.Where(rate => rate.RateDate == date)
            .Select(rate => rate.Currency)
            .ToListAsync(CancellationToken);

        return [.. Enum.GetValues<CurrencyCode>().Except(stored)];
    }
}
