using FluentValidation;
using Olevin.Api.Features.Snapshots.Currency;

namespace Olevin.Api.Features.Snapshots;

/// <summary>
/// Validation rules shared by the requests that carry snapshot amounts.
/// </summary>
public static class SnapshotValidationRules
{
    /// <summary>
    /// The largest amount, far above any real one, so that conversions can't overflow.
    /// </summary>
    private const decimal MaxAmount = 1_000_000_000_000m; // Trillion

    extension<T>(IRuleBuilder<T, Money?> ruleBuilder)
    {
        /// <summary>
        /// An amount, if given, is from 0 to <see cref="MaxAmount"/> in a supported currency.
        /// </summary>
        /// <returns>The rule builder.</returns>
        public IRuleBuilderOptions<T, Money?> MoneyRules()
        {
            return ruleBuilder
                .Must(money =>
                    money is null
                    || (money.Amount is >= 0 and <= MaxAmount && Enum.IsDefined(money.Currency))
                )
                .WithMessage("Amounts must be from 0 to 10¹² in a supported currency.");
        }
    }

    extension<T>(IRuleBuilder<T, int?> ruleBuilder)
    {
        /// <summary>
        /// The self-rating is given and is from 1 to 5.
        /// </summary>
        /// <returns>The rule builder.</returns>
        public IRuleBuilderOptions<T, int?> SelfRatingRules()
        {
            return ruleBuilder
                .NotNull()
                .WithMessage("Self-rating is required.")
                .InclusiveBetween(1, 5)
                .WithMessage("Self-rating must be from 1 to 5.");
        }
    }

    extension<T>(IRuleBuilder<T, IReadOnlyDictionary<CurrencyCode, decimal>?> ruleBuilder)
    {
        /// <summary>
        /// Rates, if given, are positive and do not include UAH, which is always 1.
        /// </summary>
        /// <returns>The rule builder.</returns>
        public IRuleBuilderOptions<T, IReadOnlyDictionary<CurrencyCode, decimal>?> FxRatesRules()
        {
            return ruleBuilder
                .Must(rates =>
                    rates is null
                    || rates.All(rate => rate.Key != CurrencyCode.UAH && rate.Value > 0)
                )
                .WithMessage("Rates must be positive and must not include UAH, which is always 1.");
        }
    }
}
