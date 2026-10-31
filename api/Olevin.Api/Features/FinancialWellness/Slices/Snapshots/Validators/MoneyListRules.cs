using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Validators;
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
            .SetValidator(new MaxEntriesValidator<T>())
            .WithMessage($"There may be at most {MaxEntries} {name}.");
        validator.RuleForEach(list).NotNull().SetValidator(new MoneyValidator());
    }

    /// <summary>
    /// Requires at most <see cref="MaxEntries"/> amounts.
    /// </summary>
    private sealed class MaxEntriesValidator<T>
        : PropertyValidator<T, IEnumerable<Money>>,
            ILengthValidator
    {
        public override string Name => nameof(MaxEntriesValidator<>);

        public int Min => 0;

        public int Max => MaxEntries;

        public override bool IsValid(ValidationContext<T> context, IEnumerable<Money> value) =>
            value is null || value.Count() <= MaxEntries;
    }
}
