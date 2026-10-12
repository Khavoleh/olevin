using Olevin.Api.Features.FinancialWellness.Seeders;
using Olevin.Api.Infrastructure.ExchangeRate;
using Olevin.Api.Shared;

namespace Olevin.Tests.Features.FinancialWellness.Seeders;

public sealed class FxRateSeederTests
{
    // 2026-10-12T10:00:00Z
    private const long UpdatedAt = 1791799200;

    private static LatestRatesResponse Response() =>
        new(
            "success",
            UpdatedAt,
            "USD",
            new Dictionary<string, decimal>
            {
                ["USD"] = 1m,
                ["EUR"] = 0.8m,
                ["UAH"] = 40m,
            }
        );

    [Fact]
    public void Rate_IsTheAmountOfCurrencyPerDollar() =>
        Assert.Equal(
            [1m, 0.8m, 40m],
            FxRateSeeder
                .ToFxRates(Response(), [CurrencyCode.USD, CurrencyCode.EUR, CurrencyCode.UAH])
                .Select(rate => rate.UnitsPerBase)
        );

    [Fact]
    public void RateDate_IsTheDateOfTheApiUpdate() =>
        Assert.All(
            FxRateSeeder.ToFxRates(Response(), [CurrencyCode.EUR]),
            rate => Assert.Equal(new DateOnly(2026, 10, 12), rate.RateDate)
        );

    [Fact]
    public void OnlyRequestedCurrencies_AreCreated() =>
        Assert.Equal(
            [CurrencyCode.UAH],
            FxRateSeeder.ToFxRates(Response(), [CurrencyCode.UAH]).Select(r => r.Currency)
        );

    [Fact]
    public void CurrencyMissingInResponse_Throws() =>
        Assert.Throws<InvalidOperationException>(() =>
            FxRateSeeder.ToFxRates(Response(), [CurrencyCode.PLN])
        );

    [Fact]
    public void FailedResult_Throws() =>
        Assert.Throws<InvalidOperationException>(() =>
            FxRateSeeder.ToFxRates(Response() with { Result = "error" }, [CurrencyCode.EUR])
        );

    [Fact]
    public void NonDollarBase_Throws() =>
        Assert.Throws<InvalidOperationException>(() =>
            FxRateSeeder.ToFxRates(Response() with { BaseCode = "EUR" }, [CurrencyCode.EUR])
        );
}
