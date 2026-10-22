using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Shared;

namespace Olevin.Tests.Features.FinancialWellness.Helpers.History;

public sealed class ExchangeRatesTests
{
    [Fact]
    public void Rates_AreTheUnitsPerDollar()
    {
        ExchangeRates rates = new(
            new Dictionary<CurrencyCode, decimal>
            {
                [CurrencyCode.USD] = 1m,
                [CurrencyCode.EUR] = 0.8m,
                [CurrencyCode.UAH] = 40m,
            }
        );

        Assert.Equal(4_000m, rates.Convert(new Money(100, CurrencyCode.USD), CurrencyCode.UAH));
        Assert.Equal(125m, rates.Convert(new Money(100, CurrencyCode.EUR), CurrencyCode.USD));
        Assert.Equal(5_000m, rates.Convert(new Money(100, CurrencyCode.EUR), CurrencyCode.UAH));
    }

    [Fact]
    public void DollarNeedsNoStoredRate() =>
        Assert.Equal(
            280m,
            new ExchangeRates(
                new Dictionary<CurrencyCode, decimal> { [CurrencyCode.UAH] = 40m }
            ).Convert(new Money(7, CurrencyCode.USD), CurrencyCode.UAH)
        );

    [Fact]
    public void UnknownCurrency_Throws() =>
        Assert.Throws<InvalidOperationException>(() =>
            new ExchangeRates(new Dictionary<CurrencyCode, decimal>()).Convert(
                new Money(1, CurrencyCode.UAH),
                CurrencyCode.USD
            )
        );
}
