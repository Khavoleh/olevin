using FluentValidation;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Snapshots;

/// <summary>
/// Validation rules shared by the requests that carry snapshot amounts.
/// </summary>
public static class SnapshotValidationRules
{
    /// <summary>
    /// The largest amount, far above any real one, so that conversions can't overflow.
    /// </summary>
    private const decimal MaxAmount = 1_000_000_000_000m; // Trillion

    /// <summary>
    /// The most reserves one snapshot may hold.
    /// </summary>
    public const int MaxReserves = 10;

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

    extension<T>(IRuleBuilder<T, IReadOnlyList<Money>?> ruleBuilder)
    {
        /// <summary>
        /// The reserves, if given, hold at most <see cref="MaxReserves"/> amounts; a currency may repeat.
        /// </summary>
        /// <returns>The rule builder.</returns>
        public IRuleBuilderOptions<T, IReadOnlyList<Money>?> ReservesRules()
        {
            return ruleBuilder
                .Must(reserves => reserves is null || reserves.Count <= MaxReserves)
                .WithMessage($"There may be at most {MaxReserves} reserves.");
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
