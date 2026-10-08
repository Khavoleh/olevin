using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Api.Features.Advice;

/// <summary>
/// Builds advice from the sensitivity analysis and turns every step into money.
/// </summary>
public static class AdviceBuilder
{
    /// <summary>
    /// Analyses all five indicators of an actual snapshot.
    /// </summary>
    /// <param name="month">The snapshot.</param>
    /// <param name="inputs">Its inputs.</param>
    /// <returns>Five rows in the order of <see cref="Indicator"/>, with ranks or reasons for filtering.</returns>
    public static IReadOnlyList<AdviceItem> Build(MonthFigures month, CalculatedInputs inputs)
    {
        AdviceItem[] rows =
        [
            .. Enum.GetValues<Indicator>().Select(indicator => Analyze(indicator, month, inputs)),
        ];

        AdviceItem[] ranked =
        [
            .. rows.Where(row => row.Rejection is null)
                .OrderBy(row => row, Comparer<AdviceItem>.Create(CompareGains)),
        ];

        return
        [
            .. rows.Select(row =>
                Array.IndexOf(ranked, row) is var place and >= 0
                    ? row with
                    {
                        Rank = place + 1,
                    }
                    : row
            ),
        ];
    }

    private static int CompareGains(AdviceItem left, AdviceItem right)
    {
        return Math.Abs(left.FuzzyGain - right.FuzzyGain) <= AdviceConstants.Tolerance
            ? left.Indicator.CompareTo(right.Indicator)
            : right.FuzzyGain.CompareTo(left.FuzzyGain);
    }

    private static AdviceItem Analyze(
        Indicator indicator,
        MonthFigures month,
        CalculatedInputs inputs
    )
    {
        Plan plan = indicator switch
        {
            Indicator.SavingsRate => CutExpenses(month),
            Indicator.Reserve => TopUpReserve(month, inputs),
            Indicator.DebtToIncome => new Plan(
                -AdviceConstants.RateStep,
                AdviceConstants.RateStep * month.Income
            ),
            Indicator.ExpenseVariation => new Plan(
                -AdviceConstants.RateStep,
                AdviceConstants.RateStep * inputs.MeanExpenses
            ),
            Indicator.SavingsRegularity => new Plan(1.0 / inputs.HistoryMonths, null),
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };

        SensitivityResult sensitivity = SensitivityAnalyzer.Analyze(
            inputs.Indicators,
            indicator,
            plan.Step
        );

        IndicatorChange Change(Indicator changed)
        {
            return new IndicatorChange(
                changed,
                sensitivity.Current[changed],
                sensitivity.Changed[changed]
            );
        }

        IndicatorChange[] changes =
            indicator == Indicator.DebtToIncome
                ? [Change(Indicator.DebtToIncome), Change(Indicator.SavingsRate)]
                : [Change(indicator)];

        return new AdviceItem(
            indicator,
            plan.Step,
            changes,
            sensitivity.FuzzyGain,
            sensitivity.LinearGain,
            plan.Amount,
            plan.IsPartial,
            plan.Cuts,
            null,
            Reject(indicator, sensitivity)
        );
    }

    private static AdviceRejection? Reject(Indicator indicator, SensitivityResult sensitivity)
    {
        double[] degrees = OlevinModel.Variable(indicator).Fuzzify(sensitivity.Current[indicator]);

        return (
            degrees[OlevinModel.BestTerm(indicator)] >= 1,
            sensitivity.FuzzyGain <= AdviceConstants.Tolerance
        ) switch
        {
            (true, _) => AdviceRejection.BestTerm,
            (_, true) => AdviceRejection.NoGain,
            _ => null,
        };
    }

    /// <summary>
    /// SR: cut flexible expenses by δ_SR · D a month. If flexible expenses are smaller, cut all of them and analyse
    /// the smaller step.
    /// </summary>
    private static Plan CutExpenses(MonthFigures month)
    {
        IReadOnlyList<(ExpenseCategory Category, double Amount)> parts =
            month.ExpenseCategories?.FlexibleParts(AdviceConstants.FoodFlexibleShare) ?? [];
        double flexible = parts.Sum(part => part.Amount);

        double step = AdviceConstants.RateStep;
        double cut = step * month.Income;
        bool isPartial = cut > flexible;

        if (isPartial)
        {
            cut = flexible;
            step = flexible / month.Income;
        }

        ExpenseCut[] cuts =
        [
            .. parts
                .Where(part => part.Amount > 0)
                .Select(part => new ExpenseCut(part.Category, cut * part.Amount / flexible))
                .OrderByDescending(part => part.Amount)
                .ThenBy(part => part.Category),
        ];

        return new Plan(step, cut, isPartial, cuts);
    }

    /// <summary>
    /// R: top up the reserve with what the surplus gives over three months, but by at most one month of
    /// obligations. Without a surplus the step is 0 and the advice is filtered out.
    /// </summary>
    private static Plan TopUpReserve(MonthFigures month, CalculatedInputs inputs)
    {
        double obligations = inputs.AverageExpenses + month.DebtPayments;
        double surplus = month.Income - month.Expenses - month.DebtPayments;
        double step =
            obligations > 0
                ? Math.Clamp(
                    AdviceConstants.ReserveTopUpMonths * surplus / obligations,
                    0,
                    AdviceConstants.MaxReserveStep
                )
                : 0;

        return new Plan(step, step * obligations);
    }

    private sealed record Plan(
        double Step,
        double? Amount,
        bool IsPartial = false,
        IReadOnlyList<ExpenseCut>? Cuts = null
    )
    {
        public IReadOnlyList<ExpenseCut> Cuts { get; } = Cuts ?? [];
    }
}
