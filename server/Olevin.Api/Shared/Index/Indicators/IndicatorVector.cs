using Olevin.Api.Shared.Index.Model;

namespace Olevin.Api.Shared.Index.Indicators;

/// <summary>
/// The vector of the five model inputs for one month.
/// </summary>
/// <param name="SavingsRate">Savings rate SR.</param>
/// <param name="Reserve">Reserve R in months.</param>
/// <param name="DebtToIncome">Debt-to-income DTI.</param>
/// <param name="ExpenseVariation">Expense variation CV.</param>
/// <param name="SavingsRegularity">Savings regularity RG.</param>
public sealed record IndicatorVector(
    double SavingsRate,
    double Reserve,
    double DebtToIncome,
    double ExpenseVariation,
    double SavingsRegularity
)
{
    /// <summary>
    /// Gets the value of one indicator.
    /// </summary>
    /// <param name="indicator">The indicator.</param>
    /// <returns>Its value.</returns>
    public double this[Indicator indicator] =>
        indicator switch
        {
            Indicator.SavingsRate => SavingsRate,
            Indicator.Reserve => Reserve,
            Indicator.DebtToIncome => DebtToIncome,
            Indicator.ExpenseVariation => ExpenseVariation,
            Indicator.SavingsRegularity => SavingsRegularity,
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };

    /// <summary>
    /// Creates a copy with one indicator replaced.
    /// </summary>
    /// <param name="indicator">The indicator to replace.</param>
    /// <param name="value">The new value.</param>
    /// <returns>The changed copy.</returns>
    public IndicatorVector With(Indicator indicator, double value)
    {
        return indicator switch
        {
            Indicator.SavingsRate => this with { SavingsRate = value },
            Indicator.Reserve => this with { Reserve = value },
            Indicator.DebtToIncome => this with { DebtToIncome = value },
            Indicator.ExpenseVariation => this with { ExpenseVariation = value },
            Indicator.SavingsRegularity => this with { SavingsRegularity = value },
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };
    }

    /// <summary>
    /// Limits every indicator to its range in the model, so that, for example, SR = −0.35 becomes −0.2.
    /// </summary>
    /// <returns>The limited copy.</returns>
    public IndicatorVector Clamp()
    {
        IndicatorVector clamped = this;

        foreach (Indicator indicator in Enum.GetValues<Indicator>())
        {
            clamped = clamped.With(
                indicator,
                OlevinModel.Variable(indicator).Clamp(this[indicator])
            );
        }

        return clamped;
    }
}
