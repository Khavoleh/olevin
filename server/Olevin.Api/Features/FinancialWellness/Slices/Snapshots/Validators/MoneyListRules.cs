using System.Linq.Expressions;
using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

/// <summary>
/// The rules of an amount entered in several currencies.
/// </summary>
public static class MoneyListRules
{
    private const int MaxEntries = 10;

    /// <summary>
    /// Requires a list of at most <see cref="MaxEntries"/> valid amounts.
    /// </summary>
    /// <typeparam name="T">The validated request.</typeparam>
    /// <param name="validator">The validator.</param>
    /// <param name="list">The list of amounts.</param>
    /// <param name="name">The name in the message.</param>
    public static void MoneyList<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, IEnumerable<Money>>> list,
        string name
    )
    {
        validator
            .RuleFor(list)
            .NotNull()
            .Must(entries => entries is null || entries.Count() <= MaxEntries)
            .WithMessage($"There may be at most {MaxEntries} {name}.");
        validator.RuleForEach(list).NotNull().SetValidator(new MoneyValidator());
    }
}
