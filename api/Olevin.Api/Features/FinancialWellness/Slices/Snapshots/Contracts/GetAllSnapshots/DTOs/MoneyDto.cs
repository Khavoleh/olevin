using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots.DTOs;

/// <summary>
/// An amount in the currency in which it was entered.
/// </summary>
/// <param name="Amount">The amount.</param>
/// <param name="Currency">The currency.</param>
public sealed record MoneyDto(decimal Amount, CurrencyCode Currency);
