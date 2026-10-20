using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Shared;

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
    /// The same six months with their calendar months, April to September 2026.
    /// </summary>
    public static SnapshotFigures[] Snapshots() =>
        [
            .. Months()
                .Select(
                    (figures, i) =>
                        new SnapshotFigures(
                            new SnapshotMonth(2026, (CalendarMonth)((int)CalendarMonth.April + i)),
                            figures
                        )
                ),
        ];

    /// <summary>
    /// The amounts of September as they are stored: reserves of 2 000 USD and 106 000 UAH and an income split into
    /// salary and other income.
    /// </summary>
    public static SnapshotAmounts SeptemberAmounts() =>
        new(
            [Uah(55_000), Uah(5_000)],
            new ExpenseAmounts(
                Uah(September.Food),
                Uah(September.Housing),
                Uah(September.Health),
                Uah(September.Clothing),
                Uah(September.Restaurants),
                Uah(September.Transport),
                Uah(September.Other)
            ),
            Uah(4_000),
            [new Money(2_000, CurrencyCode.USD), Uah(106_000)],
            [Uah(18_000)]
        );

    public static Dictionary<CurrencyCode, decimal> UnitsPerUsd =>
        new() { [CurrencyCode.UAH] = (decimal)UsdRate };

    public static Money Uah(double amount) => new((decimal)amount, CurrencyCode.UAH);
}
