using FluentValidation;

namespace Olevin.Api.Shared.Index.Indicators;

/// <summary>
/// The amounts of one monthly snapshot, already converted to the base currency.
/// </summary>
public sealed record MonthFigures
{
    private static readonly MonthFiguresValidator Validator = new();

    /// <summary>
    /// Gets a value indicating whether the snapshot is an estimate from onboarding. Estimates have only the
    /// expenses and the saved amount and take part only in CV, RG and the average expenses.
    /// </summary>
    public required bool IsEstimated { get; init; }

    /// <summary>
    /// Gets the net income D.
    /// </summary>
    public double Income { get; init; }

    /// <summary>
    /// Gets the total expenses E without debt payments.
    /// </summary>
    public required double Expenses { get; init; }

    /// <summary>
    /// Gets the expenses by category; <see langword="null"/> for an estimate.
    /// </summary>
    public ExpenseBreakdown? ExpenseCategories { get; init; }

    /// <summary>
    /// Gets the amount saved during the month S.
    /// </summary>
    public required double Saved { get; init; }

    /// <summary>
    /// Gets the liquid reserves at the end of the month L.
    /// </summary>
    public double Reserves { get; init; }

    /// <summary>
    /// Gets the monthly debt payments P.
    /// </summary>
    public double DebtPayments { get; init; }

    /// <summary>
    /// Gets the self-rating of calm from 1 to 5; 3 is neutral.
    /// </summary>
    public int SelfRating { get; init; } = IndexConstants.NeutralSelfRating;

    /// <summary>
    /// Creates an estimated snapshot from onboarding.
    /// </summary>
    /// <param name="expenses">The approximate total expenses.</param>
    /// <param name="saved">The approximate saved amount.</param>
    /// <returns>The snapshot.</returns>
    /// <exception cref="ValidationException">The amounts are not valid.</exception>
    public static MonthFigures Estimated(double expenses, double saved) =>
        Validated(
            new MonthFigures
            {
                IsEstimated = true,
                Expenses = expenses,
                Saved = saved,
            }
        );

    /// <summary>
    /// Creates an actual snapshot.
    /// </summary>
    /// <param name="income">The net income.</param>
    /// <param name="expenses">The expenses by category.</param>
    /// <param name="saved">The amount saved during the month.</param>
    /// <param name="reserves">The liquid reserves at the end of the month.</param>
    /// <param name="debtPayments">The monthly debt payments.</param>
    /// <param name="selfRating">The self-rating from 1 to 5.</param>
    /// <returns>The snapshot.</returns>
    /// <exception cref="ValidationException">The amounts or the self-rating are not valid.</exception>
    public static MonthFigures Actual(
        double income,
        ExpenseBreakdown expenses,
        double saved,
        double reserves,
        double debtPayments,
        int selfRating
    )
    {
        return Validated(
            new MonthFigures
            {
                IsEstimated = false,
                Income = income,
                Expenses = expenses.Total,
                ExpenseCategories = expenses,
                Saved = saved,
                Reserves = reserves,
                DebtPayments = debtPayments,
                SelfRating = selfRating,
            }
        );
    }

    private static MonthFigures Validated(MonthFigures month)
    {
        Validator.ValidateAndThrow(month);

        return month;
    }
}

/// <summary>
/// Validates the amounts of a snapshot: none of them is negative or not a number, and the self-rating is from 1 to 5.
/// </summary>
public sealed class MonthFiguresValidator : AbstractValidator<MonthFigures>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MonthFiguresValidator"/> class.
    /// </summary>
    public MonthFiguresValidator()
    {
        RuleFor(month => month.Income).Must(IsAmount).WithMessage(AmountMessage);
        RuleFor(month => month.Expenses).Must(IsAmount).WithMessage(AmountMessage);
        RuleFor(month => month.Saved).Must(IsAmount).WithMessage(AmountMessage);
        RuleFor(month => month.Reserves).Must(IsAmount).WithMessage(AmountMessage);
        RuleFor(month => month.DebtPayments).Must(IsAmount).WithMessage(AmountMessage);
        RuleFor(month => month.SelfRating).InclusiveBetween(1, 5);
    }

    private static string AmountMessage =>
        "'{PropertyName}' must be a finite amount of at least 0.";

    private static bool IsAmount(double amount) => double.IsFinite(amount) && amount >= 0;
}
