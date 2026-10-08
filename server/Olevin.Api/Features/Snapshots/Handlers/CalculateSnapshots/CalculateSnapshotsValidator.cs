using FluentValidation;
using Olevin.Api.Features.Snapshots.Currency;

namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// Validates the snapshots.
/// </summary>
public sealed class CalculateSnapshotsValidator : AbstractValidator<CalculateSnapshotsRequest>
{
    /// <summary>
    /// The most snapshots one request may hold: 20 years.
    /// </summary>
    public const int MaxSnapshots = 240;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalculateSnapshotsValidator"/> class.
    /// </summary>
    public CalculateSnapshotsValidator()
    {
        RuleFor(request => request.BaseCurrency).IsInEnum();

        RuleFor(request => request.Snapshots)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(snapshots => snapshots.All(snapshot => snapshot is not null))
            .WithMessage("Snapshots must not be null.")
            .Must(snapshots => snapshots.Count <= MaxSnapshots)
            .WithMessage($"There may be at most {MaxSnapshots} snapshots.")
            .Must(snapshots =>
                snapshots
                    .DistinctBy(snapshot => (snapshot.Month.Year, snapshot.Month.Month))
                    .Count() == snapshots.Count
            )
            .WithMessage("There may be only one snapshot per month.")
            .Must(snapshots => snapshots.Any(snapshot => !snapshot.IsEstimated))
            .WithMessage("At least one snapshot must be actual.");

        RuleForEach(request => request.Snapshots)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .SetValidator(new SnapshotValidator())
            .Must((request, snapshot) => HasRates(snapshot, request.BaseCurrency))
            .WithMessage(
                "Every currency of the snapshot and the base currency, except UAH, needs a rate."
            );
    }

    private static bool HasRates(SnapshotRequest snapshot, CurrencyCode baseCurrency)
    {
        ExchangeRates rates = CalculateSnapshotsMapping.Rates(snapshot);

        return rates.Has(baseCurrency)
            && CalculateSnapshotsMapping
                .Amounts(snapshot)
                .All(money => money is null || rates.Has(money.Currency));
    }

    private sealed class SnapshotValidator : AbstractValidator<SnapshotRequest>
    {
        public SnapshotValidator()
        {
            RuleFor(snapshot => snapshot.Month).NotEmpty();
            RuleFor(snapshot => snapshot.Saved).NotNull().MoneyRules();
            RuleFor(snapshot => snapshot.FxRates).FxRatesRules();

            When(
                snapshot => snapshot.IsEstimated,
                () =>
                {
                    RuleFor(snapshot => snapshot.ExpensesTotal).NotNull().MoneyRules();
                    RuleFor(snapshot => snapshot.Income).Null();
                    RuleFor(snapshot => snapshot.Expenses).Null();
                    RuleFor(snapshot => snapshot.Reserves).Null();
                    RuleFor(snapshot => snapshot.DebtPayments).Null();
                    RuleFor(snapshot => snapshot.SelfRating).Null();
                }
            );

            When(
                snapshot => !snapshot.IsEstimated,
                () =>
                {
                    RuleFor(snapshot => snapshot.ExpensesTotal).Null();
                    RuleFor(snapshot => snapshot.Income).NotNull();
                    RuleFor(snapshot => snapshot.Income!.Salary)
                        .MoneyRules()
                        .When(s => s.Income is not null);
                    RuleFor(snapshot => snapshot.Income!.Other)
                        .MoneyRules()
                        .When(s => s.Income is not null);
                    RuleFor(snapshot => snapshot.Expenses).NotNull();
                    When(
                        snapshot => snapshot.Expenses is not null,
                        () =>
                        {
                            RuleFor(snapshot => snapshot.Expenses!.Food).MoneyRules();
                            RuleFor(snapshot => snapshot.Expenses!.Housing).MoneyRules();
                            RuleFor(snapshot => snapshot.Expenses!.Health).MoneyRules();
                            RuleFor(snapshot => snapshot.Expenses!.Clothing).MoneyRules();
                            RuleFor(snapshot => snapshot.Expenses!.Restaurants).MoneyRules();
                            RuleFor(snapshot => snapshot.Expenses!.Transport).MoneyRules();
                            RuleFor(snapshot => snapshot.Expenses!.Other).MoneyRules();
                        }
                    );
                    RuleFor(snapshot => snapshot.Reserves).NotNull();
                    RuleForEach(snapshot => snapshot.Reserves).NotNull().MoneyRules();
                    RuleFor(snapshot => snapshot.DebtPayments).MoneyRules();
                    RuleFor(snapshot => snapshot.SelfRating).SelfRatingRules();
                }
            );
        }
    }
}
