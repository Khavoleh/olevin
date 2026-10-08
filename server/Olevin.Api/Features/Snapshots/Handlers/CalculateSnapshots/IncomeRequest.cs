using Olevin.Api.Features.Snapshots.Currency;

namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// The net income of a month.
/// </summary>
/// <param name="Salary">The salary.</param>
/// <param name="Other">Any other income.</param>
public sealed record IncomeRequest(Money? Salary, Money? Other);
