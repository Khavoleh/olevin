using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Data.Snapshots;

/// <summary>
/// An amount in the currency in which it was entered.
/// </summary>
/// <param name="Amount">The amount.</param>
/// <param name="Currency">The currency.</param>
public sealed record Money(decimal Amount, CurrencyCode Currency)
{
    /// <summary>
    /// Creates a zero amount, which stands for an amount that was not entered.
    /// </summary>
    /// <param name="currency">The base currency of the snapshot, so that the amount never needs a rate.</param>
    /// <returns>Zero in <paramref name="currency"/>.</returns>
    public static Money Zero(CurrencyCode currency) => new(0, currency);
}
