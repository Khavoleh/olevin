using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot.Requests;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot.Validators;

/// <summary>
/// Validates a rough month of the onboarding:.
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
