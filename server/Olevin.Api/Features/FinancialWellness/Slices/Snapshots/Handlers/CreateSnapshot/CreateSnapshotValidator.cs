using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;

/// <summary>
/// Validates a snapshot. The allowed months are checked by the endpoint.
/// </summary>
public sealed class CreateSnapshotValidator : AbstractValidator<CreateSnapshotRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSnapshotValidator"/> class.
    /// </summary>
    public CreateSnapshotValidator()
    {
        RuleFor(request => request.Year)
            .InclusiveBetween((short)2000, (short)9999)
            .WithMessage("Year must be from 2000 to 9999.");
        RuleFor(request => request.Month).IsInEnum().WithMessage("The month is not valid.");

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
