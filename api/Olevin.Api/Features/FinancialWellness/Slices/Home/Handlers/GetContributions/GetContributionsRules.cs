using System.Numerics;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions.Responses;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions;

/// <summary>
/// The rules of the contributions to the change of the index: the Shapley values of the inputs.
/// </summary>
public static class GetContributionsRules
{
    private static readonly Indicator[] Indicators = Enum.GetValues<Indicator>();

    /// <summary>
    /// Finds the snapshot before the latest one.
    /// </summary>
    /// <param name="months">The history in month order, estimates included; the last one is the latest snapshot.</param>
    /// <returns>Its position, or −1 when the latest snapshot is the first one.</returns>
    public static int PreviousSnapshot(IReadOnlyList<MonthFigures> months) =>
        months.Take(months.Count - 1).ToList().FindLastIndex(month => !month.IsEstimated);

    /// <summary>
    /// Splits the change of the fuzzy index between the indicators.
    /// </summary>
    /// <param name="previous">The indicators of the previous snapshot.</param>
    /// <param name="latest">The indicators of the latest snapshot.</param>
    /// <returns>
    /// The gain of replacing each indicator with its latest value, averaged over every order of the replacements;
    /// together they make the whole change of I_fuzzy.
    /// </returns>
    public static IReadOnlyDictionary<Indicator, double> IndicatorPoints(
        IndicatorVector previous,
        IndicatorVector latest
    )
    {
        double[] fuzzy =
        [
            .. Enumerable
                .Range(0, 1 << Indicators.Length)
                .Select(subset => OlevinModel.Evaluate(Mixed(previous, latest, subset)).Value),
        ];

        return Indicators
            .Select((indicator, position) => (indicator, bit: 1 << position))
            .ToDictionary(
                item => item.indicator,
                item =>
                    Enumerable
                        .Range(0, fuzzy.Length)
                        .Where(subset => (subset & item.bit) == 0)
                        .Sum(subset => Weight(subset) * (fuzzy[subset | item.bit] - fuzzy[subset]))
            );
    }

    /// <summary>
    /// Gets how much the self-rating changed the index.
    /// </summary>
    /// <param name="previousFuzzy">I_fuzzy of the previous snapshot.</param>
    /// <param name="previousRating">The self-rating of the previous snapshot.</param>
    /// <param name="latestFuzzy">I_fuzzy of the latest snapshot.</param>
    /// <param name="latestRating">The self-rating of the latest snapshot.</param>
    /// <returns>The difference of the corrections the two snapshots got after limiting to 0–100.</returns>
    public static double SelfRatingPoints(
        double previousFuzzy,
        int previousRating,
        double latestFuzzy,
        int latestRating
    ) => Correction(latestFuzzy, latestRating) - Correction(previousFuzzy, previousRating);

    /// <summary>
    /// Rounds values to one decimal so that they add up to a rounded total.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="total">The total with one decimal.</param>
    /// <returns>Each value rounded up or down, the ones closest to the other side moved first.</returns>
    public static IReadOnlyList<double> RoundToTotal(IReadOnlyList<double> values, double total)
    {
        long[] tenths = [.. values.Select(Tenths)];
        long gap = Tenths(total) - tenths.Sum();

        while (gap != 0)
        {
            int step = Math.Sign(gap);
            int moved = Enumerable
                .Range(0, values.Count)
                .MaxBy(index => ((values[index] * 10) - tenths[index]) * step);

            tenths[moved] += step;
            gap -= step;
        }

        return [.. tenths.Select(value => value / 10.0)];
    }

    /// <summary>
    /// Gets the contributions of every factor.
    /// </summary>
    /// <param name="previousMonth">The previous snapshot.</param>
    /// <param name="previous">Its inputs.</param>
    /// <param name="latestMonth">The latest snapshot.</param>
    /// <param name="latest">Its inputs.</param>
    /// <param name="change">The change of the shown index the contributions add up to.</param>
    /// <returns>The contributions rounded to one decimal.</returns>
    public static GetContributionsResponse Contributions(
        MonthFigures previousMonth,
        CalculatedInputs previous,
        MonthFigures latestMonth,
        CalculatedInputs latest,
        double change
    )
    {
        IReadOnlyDictionary<Indicator, double> indicatorPoints = IndicatorPoints(
            previous.Indicators,
            latest.Indicators
        );
        double selfRatingPoints = SelfRatingPoints(
            OlevinModel.Evaluate(previous.Indicators).Value,
            previousMonth.SelfRating,
            OlevinModel.Evaluate(latest.Indicators).Value,
            latestMonth.SelfRating
        );
        IReadOnlyList<double> points = RoundToTotal(
            [.. Indicators.Select(indicator => indicatorPoints[indicator]), selfRatingPoints],
            change
        );

        IndicatorContributionResponse Contribution(Indicator indicator) =>
            new(
                points[(int)indicator],
                Shown(indicator, previous, previousMonth),
                Shown(indicator, latest, latestMonth)
            );

        return new GetContributionsResponse(
            Contribution(Indicator.SavingsRate),
            Contribution(Indicator.Reserve),
            Contribution(Indicator.DebtToIncome),
            Contribution(Indicator.ExpenseVariation),
            new SavingsRegularityContributionResponse(
                points[(int)Indicator.SavingsRegularity],
                previous.SavingMonths,
                previous.HistoryMonths,
                latest.SavingMonths,
                latest.HistoryMonths
            ),
            new SelfRatingContributionResponse(
                points[^1],
                previousMonth.SelfRating,
                latestMonth.SelfRating
            )
        );
    }

    private static IndicatorVector Mixed(
        IndicatorVector previous,
        IndicatorVector latest,
        int subset
    ) =>
        Indicators
            .Where((_, position) => (subset & (1 << position)) != 0)
            .Aggregate(previous, (vector, indicator) => vector.With(indicator, latest[indicator]));

    private static double Weight(int subset)
    {
        int size = BitOperations.PopCount((uint)subset);

        return Factorial(size)
            * Factorial(Indicators.Length - size - 1)
            / Factorial(Indicators.Length);
    }

    private static double Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);

    private static double Correction(double fuzzy, int selfRating) =>
        IndexCalculator.AdjustForSelfRating(fuzzy, selfRating) - fuzzy;

    private static long Tenths(double value) =>
        (long)Math.Round(value * 10, MidpointRounding.AwayFromZero);

    private static double? Shown(
        Indicator indicator,
        CalculatedInputs inputs,
        MonthFigures month
    ) =>
        indicator switch
        {
            Indicator.SavingsRate => ShownIndicators.OfIncome(inputs.Raw.SavingsRate, month),
            Indicator.DebtToIncome => ShownIndicators.OfIncome(inputs.Raw.DebtToIncome, month),
            Indicator.Reserve => ShownIndicators.ReserveMonths(inputs, month),
            Indicator.ExpenseVariation => ShownIndicators.ExpenseVariation(inputs),
            Indicator.SavingsRegularity => inputs.Raw.SavingsRegularity,
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };
}
