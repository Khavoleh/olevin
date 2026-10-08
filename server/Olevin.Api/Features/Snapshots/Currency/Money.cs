namespace Olevin.Api.Features.Snapshots.Currency;

/// <summary>
/// An amount in the currency in which it was entered.
/// </summary>
/// <param name="Amount">The amount.</param>
/// <param name="Currency">The currency.</param>
public sealed record Money(decimal Amount, CurrencyCode Currency);
