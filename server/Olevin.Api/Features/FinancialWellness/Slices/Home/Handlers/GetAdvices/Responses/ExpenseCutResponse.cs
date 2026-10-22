using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// The part of the cut of expenses that falls on one category.
/// </summary>
/// <param name="Category">The category.</param>
/// <param name="Amount">The monthly amount, rounded to cents.</param>
public sealed record ExpenseCutResponse(ExpenseCategory Category, double Amount);
