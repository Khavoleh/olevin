using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.RescoreSnapshots;

/// <summary>
/// Asks to score every snapshot of the user again in a new base currency.
/// </summary>
/// <param name="UserId">The identifier of the user.</param>
/// <param name="BaseCurrency">The new base currency of the user.</param>
public sealed record RescoreSnapshotsRequest(Guid UserId, CurrencyCode BaseCurrency);
