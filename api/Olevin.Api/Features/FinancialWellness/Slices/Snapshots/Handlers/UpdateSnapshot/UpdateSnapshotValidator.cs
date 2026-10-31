using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.UpdateSnapshot;

/// <summary>
/// Validates the new values of a snapshot. Its existence is checked by the endpoint.
/// </summary>
public sealed class UpdateSnapshotValidator : AbstractValidator<UpdateSnapshotRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSnapshotValidator"/> class.
    /// </summary>
    public UpdateSnapshotValidator()
    {
        this.MoneyList(request => request.Income, "income");
        RuleFor(request => request.Expenses).NotNull().SetValidator(new ExpensesRequestValidator());
        RuleFor(request => request.Saved).NotNull().SetValidator(new MoneyValidator());
        this.MoneyList(request => request.Reserves, "reserves");
        this.MoneyList(request => request.DebtPayments, "debt payments");

        RuleFor(request => request.SelfRating)
            .InclusiveBetween(1, 5)
            .WithMessage("Self-rating must be from 1 to 5.");
    }
}
