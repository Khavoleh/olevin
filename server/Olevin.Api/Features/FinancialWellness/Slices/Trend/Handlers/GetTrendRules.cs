using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Api.Features.FinancialWellness.Slices.Trend.Handlers;

/// <summary>
/// The rules of the trend.
/// </summary>
public static class GetTrendRules
{
    private const int MonthsInYear = 12;

    /// <summary>
    /// Lists every month of the period with its snapshot.
    /// </summary>
    /// <param name="period">The period chosen by the user.</param>
    /// <param name="latestOpen">The latest month whose snapshot day has come.</param>
    /// <param name="snapshots">Every snapshot of the user, at least one, in any order.</param>
    /// <returns>
    /// The months from the start of the period, or from the first snapshot when it is later, up to
    /// <paramref name="latestOpen"/> or the latest snapshot when it is later, the oldest first; empty when no snapshot
    /// falls into the period.
    /// </returns>
    public static IReadOnlyList<TrendPointResponse> Points(
        TrendPeriod period,
        SnapshotMonth latestOpen,
        IReadOnlyList<SnapshotDto> snapshots
    )
    {
        Dictionary<int, SnapshotDto> byMonth = snapshots.ToDictionary(snapshot =>
            Ordinal(snapshot.Year, snapshot.Month)
        );
        int earliest = byMonth.Keys.Min();
        int last = Math.Max(Ordinal(latestOpen.Year, latestOpen.Month), byMonth.Keys.Max());
        int start = period switch
        {
            TrendPeriod.SixMonths => last - 5,
            TrendPeriod.TwelveMonths => last - 11,
            TrendPeriod.All => earliest,
            _ => throw new ArgumentOutOfRangeException(nameof(period), period, null),
        };
        int first = Math.Max(start, earliest);

        return !byMonth.Keys.Any(month => month >= first)
            ? []
            :
            [
                .. Enumerable
                    .Range(first, last - first + 1)
                    .Select(month =>
                    {
                        SnapshotMonth calendar = MonthOf(month);

                        return byMonth.TryGetValue(month, out SnapshotDto? snapshot)
                            ? new TrendPointResponse(
                                calendar.Year,
                                calendar.Month,
                                IndexPoints.Round(snapshot.FinalIndex),
                                IndexPoints.Round(snapshot.CashFlow),
                                IndexPoints.Round(snapshot.Buffer)
                            )
                            : new TrendPointResponse(
                                calendar.Year,
                                calendar.Month,
                                null,
                                null,
                                null
                            );
                    }),
            ];
    }

    private static int Ordinal(short year, CalendarMonth month) =>
        (year * MonthsInYear) + (int)month - 1;

    private static SnapshotMonth MonthOf(int ordinal) =>
        new((short)(ordinal / MonthsInYear), (CalendarMonth)((ordinal % MonthsInYear) + 1));
}
