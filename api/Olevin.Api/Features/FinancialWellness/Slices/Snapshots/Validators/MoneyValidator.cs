using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Validators;

/// <summary>
/// Validates an amount from 0 to <see cref="MaxAmount"/> in a supported currency.
/// </summary>
public sealed class MoneyValidator : AbstractValidator<Money>
{
    private const decimal MaxAmount = 1_000_000_000_000m; // Trillion

    /// <summary>
    /// Initializes a new instance of the <see cref="MoneyValidator"/> class.
    /// </summary>
    public MoneyValidator()
    {
        RuleFor(money => money.Amount)
            .InclusiveBetween(0, MaxAmount)
            .WithMessage("Amounts must be from 0 to 10¹².");
        RuleFor(money => money.Currency).IsInEnum().WithMessage("The currency is not supported.");
    }
}
