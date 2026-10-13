namespace Olevin.Api.Features.FinancialWellness.Data.Snapshots;

/// <summary>
/// The net income of the month as entered, the <c>income</c> part of <c>snapshots.amounts</c>. An amount that was not
/// entered is <see cref="Money.Zero"/> in the base currency.
/// </summary>
/// <param name="Salary">The salary.</param>
/// <param name="Other">Any other income.</param>
public sealed record IncomeAmounts(Money Salary, Money Other);
