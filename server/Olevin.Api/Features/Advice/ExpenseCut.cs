using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Features.Advice;

/// <summary>
/// The part of a cut of expenses that falls on one category.
/// </summary>
/// <param name="Category">The category.</param>
/// <param name="Amount">The monthly amount in the base currency.</param>
public sealed record ExpenseCut(ExpenseCategory Category, double Amount);
