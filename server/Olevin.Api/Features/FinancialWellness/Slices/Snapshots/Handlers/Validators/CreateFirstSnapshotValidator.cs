using System.Linq.Expressions;
using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.Requests;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.Validators;

/// <summary>
/// Validates the first snapshot and its estimates.
/// </summary>
public sealed class CreateFirstSnapshotValidator : AbstractValidator<CreateFirstSnapshotRequest>
{
    private const int MaxMoneyEntries = 10;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFirstSnapshotValidator"/> class.
    /// </summary>
    public CreateFirstSnapshotValidator()
    {
        MoneyList(request => request.Income, "income");
        RuleFor(request => request.Expenses).NotNull().SetValidator(new ExpensesRequestValidator());
        RuleFor(request => request.Saved).NotNull().SetValidator(new MoneyValidator());
        MoneyList(request => request.Reserves, "reserves");
        MoneyList(request => request.DebtPayments, "debt payments");

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

    private void MoneyList(
        Expression<Func<CreateFirstSnapshotRequest, IEnumerable<Money>>> list,
        string name
    )
    {
        RuleFor(list)
            .NotNull()
            .Must(entries => entries is null || entries.Count() <= MaxMoneyEntries)
            .WithMessage($"There may be at most {MaxMoneyEntries} {name}.");
        RuleForEach(list).NotNull().SetValidator(new MoneyValidator());
    }
}
