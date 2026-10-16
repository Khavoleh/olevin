using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot.Requests;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot.Validators;

/// <summary>
/// Validates the first snapshot and its estimates.
/// </summary>
public sealed class CreateFirstSnapshotValidator : AbstractValidator<CreateFirstSnapshotRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFirstSnapshotValidator"/> class.
    /// </summary>
    public CreateFirstSnapshotValidator()
    {
        this.MoneyList(request => request.Income, "income");
        RuleFor(request => request.Expenses).NotNull().SetValidator(new ExpensesRequestValidator());
        RuleFor(request => request.Saved).NotNull().SetValidator(new MoneyValidator());
        this.MoneyList(request => request.Reserves, "reserves");
        this.MoneyList(request => request.DebtPayments, "debt payments");

        RuleFor(request => request.SelfRating)
            .InclusiveBetween(1, 5)
            .WithMessage("Self-rating must be from 1 to 5.");

        RuleFor(request => request.PreviousMonth)
            .NotNull()
            .SetValidator(new EstimateRequestValidator());
        RuleFor(request => request.TwoMonthsAgo)
            .NotNull()
            .SetValidator(new EstimateRequestValidator());
    }
}
