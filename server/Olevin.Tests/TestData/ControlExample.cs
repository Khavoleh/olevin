using Olevin.Api.Features.Snapshots.Currency;
using Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;
using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Tests.TestData;

/// <summary>
/// The control example: six actual snapshots in UAH.
/// </summary>
internal static class ControlExample
{
    public const double UsdRate = 41.50;

    public static readonly double[] Expenses = [28_000, 46_000, 28_000, 30_000, 42_000, 36_000];

    public static readonly double[] Saved = [6_000, 0, 0, 5_000, 0, 4_000];

    public static ExpenseBreakdown September { get; } =
        new(
            Food: 9_000,
            Housing: 12_000,
            Health: 1_500,
            Clothing: 2_500,
            Restaurants: 3_000,
            Transport: 2_500,
            Other: 5_500
        );

    /// <summary>
    /// April to September. Only the expenses and savings of earlier months matter for September.
    /// </summary>
    public static MonthFigures[] Months()
    {
        MonthFigures[] months = new MonthFigures[Expenses.Length];

        for (int i = 0; i < months.Length - 1; i++)
        {
            months[i] = MonthFigures.Actual(
                60_000,
                new ExpenseBreakdown(0, 0, 0, 0, 0, 0, Expenses[i]),
                Saved[i],
                150_000,
                18_000,
                3
            );
        }

        months[^1] = MonthFigures.Actual(
            60_000,
            September,
            4_000,
            (2_000 * UsdRate) + 106_000,
            18_000,
            4
        );

        return months;
    }

    /// <summary>
    /// The same months as a request: April and May are onboarding estimates, June to August are actual, September
    /// has reserves of 2 000 USD and 106 000 UAH and an income split into salary and other income.
    /// </summary>
    public static CalculateSnapshotsRequest Request()
    {
        DateOnly[] months = [.. Enumerable.Range(4, 6).Select(m => new DateOnly(2026, m, 1))];
        SnapshotRequest[] snapshots = new SnapshotRequest[6];

        for (int i = 0; i < 2; i++)
            snapshots[i] = EstimatedRequest(months[i], Expenses[i], Saved[i]);

        for (int i = 2; i < 5; i++)
        {
            snapshots[i] = new SnapshotRequest(
                months[i],
                false,
                null,
                null,
                Uah(Saved[i]),
                new IncomeRequest(Uah(60_000), null),
                new ExpensesRequest(null, null, null, null, null, null, Uah(Expenses[i])),
                [Uah(150_000)],
                Uah(18_000),
                3
            );
        }

        snapshots[5] = new SnapshotRequest(
            months[5],
            false,
            UsdRates,
            null,
            Uah(4_000),
            new IncomeRequest(Uah(55_000), Uah(5_000)),
            new ExpensesRequest(
                Uah(September.Food),
                Uah(September.Housing),
                Uah(September.Health),
                Uah(September.Clothing),
                Uah(September.Restaurants),
                Uah(September.Transport),
                Uah(September.Other)
            ),
            [new Money(2_000, CurrencyCode.USD), Uah(106_000)],
            Uah(18_000),
            4
        );

        return new CalculateSnapshotsRequest(CurrencyCode.UAH, snapshots);
    }

    public static Dictionary<CurrencyCode, decimal> UsdRates =>
        new() { [CurrencyCode.USD] = (decimal)UsdRate };

    public static SnapshotRequest EstimatedRequest(DateOnly month, double expenses, double saved)
    {
        return new SnapshotRequest(
            month,
            true,
            null,
            Uah(expenses),
            Uah(saved),
            null,
            null,
            null,
            null,
            null
        );
    }

    public static Money Uah(double amount) => new((decimal)amount, CurrencyCode.UAH);
}
