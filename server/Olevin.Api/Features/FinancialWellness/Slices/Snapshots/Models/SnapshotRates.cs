namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;

/// <summary>
/// The rates a snapshot is converted at.
/// </summary>
/// <param name="FxDate">The date the rates are valid for.</param>
/// <param name="Rates">The rates.</param>
public sealed record SnapshotRates(DateOnly FxDate, ExchangeRates Rates);
