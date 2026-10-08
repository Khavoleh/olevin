namespace Olevin.Api.Shared.Index.Indicators;

/// <summary>
/// Calculates the five model inputs of a snapshot from it and the snapshots before it.
/// </summary>
public static class InputsCalculator
{
    /// <summary>
    /// SR when there is no income.
    /// </summary>
    private const double NoIncomeSavingsRate = -0.2;

    /// <summary>
    /// DTI when there is no income but there are debt payments.
    /// </summary>
    private const double NoIncomeDebtToIncome = 0.6;

    /// <summary>
    /// R when there are reserves but no expenses or debt payments.
    /// </summary>
    private const double NoExpensesReserve = 9;

    /// <summary>
    /// Calculates the inputs of one snapshot.
    /// </summary>
    /// <param name="months">All snapshots in month order, estimates included.</param>
    /// <param name="index">The position of the snapshot in <paramref name="months"/>.</param>
    /// <returns>The inputs and the values they are calculated from.</returns>
    public static CalculatedInputs Calculate(IReadOnlyList<MonthFigures> months, int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, months.Count);

        MonthFigures month = months[index];
        int available = index + 1;

        double averageExpenses = Mean(
            Window(months, index, IndexConstants.AverageWindow).Select(m => m.Expenses)
        );

        IReadOnlyList<MonthFigures> history = Window(months, index, IndexConstants.HistoryWindow);
        double[] expenses = [.. history.Select(m => m.Expenses)];
        double meanExpenses = Mean(expenses);
        double deviation = Math.Sqrt(
            Mean(expenses.Select(e => (e - meanExpenses) * (e - meanExpenses)))
        );
        int savingMonths = history.Count(m => m.Saved > 0);

        double savingsRate;
        double debtToIncome;

        if (month.Income > 0)
        {
            savingsRate = (month.Income - month.Expenses - month.DebtPayments) / month.Income;
            debtToIncome = month.DebtPayments / month.Income;
        }
        else
        {
            savingsRate = NoIncomeSavingsRate;
            debtToIncome = month.DebtPayments > 0 ? NoIncomeDebtToIncome : 0;
        }

        double obligations = averageExpenses + month.DebtPayments;
        double reserve = (obligations > 0, month.Reserves > 0) switch
        {
            (true, _) => month.Reserves / obligations,
            (_, true) => NoExpensesReserve,
            _ => 0,
        };

        double variation = meanExpenses > 0 ? deviation / meanExpenses : 0;
        double regularity = (double)savingMonths / history.Count;

        IndicatorVector raw = new(savingsRate, reserve, debtToIncome, variation, regularity);

        return new CalculatedInputs(
            raw.Clamp(),
            raw,
            available,
            averageExpenses,
            meanExpenses,
            deviation,
            savingMonths,
            history.Count
        );
    }

    private static List<MonthFigures> Window(
        IReadOnlyList<MonthFigures> months,
        int index,
        int size
    )
    {
        int start = Math.Max(0, index + 1 - size);

        return [.. months.Skip(start).Take(index + 1 - start)];
    }

    private static double Mean(IEnumerable<double> values)
    {
        double[] array = [.. values];

        return array.Length > 0 ? array.Sum() / array.Length : 0;
    }
}
