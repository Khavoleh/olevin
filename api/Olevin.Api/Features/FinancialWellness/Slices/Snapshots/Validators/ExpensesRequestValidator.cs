using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

/// <summary>
/// Validates the expenses of a month.
/// </summary>
public sealed class ExpensesRequestValidator : AbstractValidator<ExpensesRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpensesRequestValidator"/> class.
    /// </summary>
    public ExpensesRequestValidator()
    {
        RuleFor(expenses => expenses.Food).NotNull().SetValidator(new MoneyValidator());
        RuleFor(expenses => expenses.Housing).NotNull().SetValidator(new MoneyValidator());
        RuleFor(expenses => expenses.Health).NotNull().SetValidator(new MoneyValidator());
        RuleFor(expenses => expenses.Clothing).NotNull().SetValidator(new MoneyValidator());
        RuleFor(expenses => expenses.Restaurants).NotNull().SetValidator(new MoneyValidator());
        RuleFor(expenses => expenses.Transport).NotNull().SetValidator(new MoneyValidator());
        RuleFor(expenses => expenses.Other).NotNull().SetValidator(new MoneyValidator());
    }
}
