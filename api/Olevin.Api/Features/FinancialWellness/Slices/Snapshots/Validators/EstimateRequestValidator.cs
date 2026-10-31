using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

/// <summary>
/// Validates a rough month before the first snapshot.
/// </summary>
public sealed class EstimateRequestValidator : AbstractValidator<EstimateRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EstimateRequestValidator"/> class.
    /// </summary>
    public EstimateRequestValidator()
    {
        RuleFor(estimate => estimate.ExpensesTotal).NotNull().SetValidator(new MoneyValidator());
        RuleFor(estimate => estimate.Saved).NotNull().SetValidator(new MoneyValidator());
    }
}
