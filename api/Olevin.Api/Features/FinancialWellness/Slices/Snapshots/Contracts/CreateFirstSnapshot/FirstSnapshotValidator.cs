using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.CreateFirstSnapshot;

/// <summary>
/// Validates the first snapshot and its estimates.
/// </summary>
public sealed class FirstSnapshotValidator : AbstractValidator<FirstSnapshot>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirstSnapshotValidator"/> class.
    /// </summary>
    public FirstSnapshotValidator()
    {
        this.MoneyList(snapshot => snapshot.Income, "income");
        RuleFor(snapshot => snapshot.Expenses)
            .NotNull()
            .SetValidator(new ExpensesRequestValidator());
        RuleFor(snapshot => snapshot.Saved).NotNull().SetValidator(new MoneyValidator());
        this.MoneyList(snapshot => snapshot.Reserves, "reserves");
        this.MoneyList(snapshot => snapshot.DebtPayments, "debt payments");

        RuleFor(snapshot => snapshot.SelfRating)
            .InclusiveBetween(1, 5)
            .WithMessage("Self-rating must be from 1 to 5.");

        RuleFor(snapshot => snapshot.PreviousMonth)
            .NotNull()
            .SetValidator(new EstimateRequestValidator());
        RuleFor(snapshot => snapshot.TwoMonthsAgo)
            .NotNull()
            .SetValidator(new EstimateRequestValidator());
    }
}
