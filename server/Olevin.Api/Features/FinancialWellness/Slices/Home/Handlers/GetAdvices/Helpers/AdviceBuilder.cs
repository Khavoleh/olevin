using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Enums;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;

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
        double fuzzy = OlevinModel.Evaluate(inputs.Indicators).Value;
        AdviceItem[] rows =
        [
            .. Enum.GetValues<Indicator>()
                .Select(indicator => Analyze(indicator, month, inputs, fuzzy)),
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
        CalculatedInputs inputs,
        double fuzzy
    )
    {
        Plan plan = indicator switch
        {
            Indicator.SavingsRate or Indicator.DebtToIncome or Indicator.ExpenseVariation =>
                RateStep(indicator, month, inputs, AdviceConstants.RateStep),
            Indicator.Reserve => TopUpReserve(month, inputs),
            Indicator.SavingsRegularity => new Plan(1.0 / inputs.HistoryMonths, null),
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };

        IndicatorVector start = inputs.Indicators;
        SensitivityResult sensitivity = SensitivityAnalyzer.Analyze(start, indicator, plan.Step);
        AdviceRejection? rejection = Reject(indicator, sensitivity);

        if (rejection == AdviceRejection.NoGain && Extend(indicator, month, inputs) is { } extended)
        {
            (plan, sensitivity) = extended;
            start = inputs.Raw;
            rejection = null;
        }

        double finalGain =
            IndexCalculator.AdjustForSelfRating(fuzzy + sensitivity.FuzzyGain, month.SelfRating)
            - IndexCalculator.AdjustForSelfRating(fuzzy, month.SelfRating);

        if (rejection is null && finalGain <= AdviceConstants.Tolerance)
            rejection = AdviceRejection.NoGain;

        IndicatorChange Change(Indicator changed) =>
            new(changed, start[changed], sensitivity.Changed[changed]);

        IndicatorChange[] changes =
            indicator == Indicator.DebtToIncome
                ? [Change(Indicator.DebtToIncome), Change(Indicator.SavingsRate)]
                : [Change(indicator)];

        return new AdviceItem(
            indicator,
            plan.Step,
            changes,
            sensitivity.FuzzyGain,
            finalGain,
            plan.Amount,
            plan.IsPartial,
            plan.Cuts,
            null,
            rejection
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
    /// When the usual step of SR, DTI or CV does not raise I_fuzzy, tries larger steps from the raw value up to what
    /// the person can do, and keeps the smallest one that gives <see cref="AdviceConstants.ExtendedGain"/>, or the
    /// largest gain within reach when that is less.
    /// </summary>
    private static (Plan Plan, SensitivityResult Sensitivity)? Extend(
        Indicator indicator,
        MonthFigures month,
        CalculatedInputs inputs
    )
    {
        double limit = MaxStep(indicator, month, inputs);
        double direction = indicator == Indicator.SavingsRate ? 1 : -1;
        double size = AdviceConstants.RateStep;
        List<(double Size, SensitivityResult Sensitivity)> steps = [];

        while (size < limit)
        {
            size = Math.Min(size + AdviceConstants.SearchStep, limit);
            steps.Add((size, SensitivityAnalyzer.Analyze(inputs.Raw, indicator, direction * size)));
        }

        double reachable = steps.Count > 0 ? steps.Max(step => step.Sensitivity.FuzzyGain) : 0;

        if (reachable <= AdviceConstants.Tolerance)
            return null;

        double target = Math.Min(AdviceConstants.ExtendedGain, reachable);
        (double chosen, SensitivityResult sensitivity) = steps.First(step =>
            step.Sensitivity.FuzzyGain >= target - AdviceConstants.Tolerance
        );

        return (RateStep(indicator, month, inputs, chosen), sensitivity);
    }

    /// <summary>
    /// The largest step the person can make: all flexible expenses for SR, all debt payments for DTI and the whole
    /// variation for CV; 0 for the other indicators and without income for SR and DTI.
    /// </summary>
    private static double MaxStep(Indicator indicator, MonthFigures month, CalculatedInputs inputs)
    {
        return indicator switch
        {
            Indicator.SavingsRate => month.Income > 0
                ? FlexibleParts(month).Sum(part => part.Amount) / month.Income
                : 0,
            Indicator.DebtToIncome => month.Income > 0 ? inputs.Raw.DebtToIncome : 0,
            Indicator.ExpenseVariation => inputs.Raw.ExpenseVariation,
            Indicator.Reserve or Indicator.SavingsRegularity => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };
    }

    private static Plan RateStep(
        Indicator indicator,
        MonthFigures month,
        CalculatedInputs inputs,
        double size
    )
    {
        return indicator switch
        {
            Indicator.SavingsRate => CutExpenses(month, size),
            Indicator.DebtToIncome => new Plan(-size, size * month.Income),
            Indicator.ExpenseVariation => new Plan(-size, size * inputs.MeanExpenses),
            Indicator.Reserve or Indicator.SavingsRegularity =>
                throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };
    }

    /// <summary>
    /// SR: cut flexible expenses by δ_SR · D a month. If flexible expenses are smaller, cut all of them and analyse
    /// the smaller step.
    /// </summary>
    private static Plan CutExpenses(MonthFigures month, double size)
    {
        IReadOnlyList<(ExpenseCategory Category, double Amount)> parts = FlexibleParts(month);
        double flexible = parts.Sum(part => part.Amount);

        double step = size;
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

    private static IReadOnlyList<(ExpenseCategory Category, double Amount)> FlexibleParts(
        MonthFigures month
    ) => month.ExpenseCategories?.FlexibleParts(AdviceConstants.FoodFlexibleShare) ?? [];

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
