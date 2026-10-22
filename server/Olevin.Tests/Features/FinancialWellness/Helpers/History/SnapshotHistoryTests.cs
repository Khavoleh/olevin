using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Helpers.History;

public sealed class SnapshotHistoryTests
{
    private static readonly DateOnly FxDate = new(2026, 9, 25);

    [Fact]
    public void Build_OrdersEstimatesAndSnapshotsByMonth()
    {
        EstimatedSnapshot[] estimates = [Estimate(CalendarMonth.June), Estimate(CalendarMonth.May)];
        Snapshot[] snapshots =
        [
            Stored(2026, CalendarMonth.September),
            Stored(2026, CalendarMonth.July),
        ];

        SnapshotFigures[] history = SnapshotHistory.Build(
            estimates,
            snapshots,
            new Dictionary<DateOnly, ExchangeRates> { [FxDate] = new(ControlExample.UnitsPerUsd) },
            CurrencyCode.UAH
        );

        Assert.Equal(
            [CalendarMonth.May, CalendarMonth.June, CalendarMonth.July, CalendarMonth.September],
            history.Select(figures => figures.Month.Month)
        );
        Assert.Equal(
            [true, true, false, false],
            history.Select(figures => figures.Figures.IsEstimated)
        );
    }

    private static EstimatedSnapshot Estimate(CalendarMonth month) =>
        new()
        {
            Year = 2026,
            Month = month,
            FxDate = FxDate,
            ExpensesCurrency = CurrencyCode.UAH,
            ExpensesAmount = 30_000,
            SavedCurrency = CurrencyCode.UAH,
            SavedAmount = 5_000,
        };

    private static Snapshot Stored(short year, CalendarMonth month) =>
        new()
        {
            Year = year,
            Month = month,
            FxDate = FxDate,
            SelfRating = 3,
            Amounts = ControlExample.SeptemberAmounts(),
        };
}
