using Olevin.Api.Features.FinancialWellness.Data.Snapshots;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;

/// <summary>
/// A rough month before the first snapshot, entered at onboarding.
/// </summary>
/// <param name="ExpensesTotal">The approximate total expenses of the month.</param>
/// <param name="Saved">The amount saved during the month.</param>
public sealed record EstimateRequest(Money ExpensesTotal, Money Saved);
