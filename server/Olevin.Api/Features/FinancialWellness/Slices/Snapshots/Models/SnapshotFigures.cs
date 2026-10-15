using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;

/// <summary>
/// The amounts of one month of the user's history, already converted to the base currency.
/// </summary>
/// <param name="Month">The month the amounts belong to.</param>
/// <param name="Figures">The amounts, actual or estimated.</param>
public sealed record SnapshotFigures(SnapshotMonth Month, MonthFigures Figures);
