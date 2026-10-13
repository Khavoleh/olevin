using Olevin.Api.Features.FinancialWellness.Data.Snapshots;

namespace Olevin.Api.Features.FinancialWellness.Handlers.CalculateSnapshots;

/// <summary>
/// The net income of a month.
/// </summary>
/// <param name="Salary">The salary.</param>
/// <param name="Other">Any other income.</param>
public sealed record IncomeRequest(Money? Salary, Money? Other);
