using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshots;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshots;

public sealed class GetSnapshotsRulesTests
{
    private const double Precision = 1e-9;
    private const short SnapshotDay = 31;
    private static readonly SnapshotMonth Earliest = new(2026, CalendarMonth.August);
    private static readonly SnapshotMonth Latest = new(2026, CalendarMonth.October);

    [Fact]
    public void ChangeAt_IsTheDifferenceOfTheShownIndexes()
    {
        double[] newestFirst = [58.66, 55.14, 50.2];

        Assert.Equal(3.6, GetSnapshotsRules.ChangeAt(newestFirst, 0)!.Value, Precision);
        Assert.Equal(4.9, GetSnapshotsRules.ChangeAt(newestFirst, 1)!.Value, Precision);
    }

    [Fact]
    public void ChangeAt_OfTheFirstSnapshot_IsNull() =>
        Assert.Null(GetSnapshotsRules.ChangeAt([58.66, 55.14], 1));

    [Fact]
    public void RoundMoney_KeepsTwoDecimals() =>
        Assert.Equal(1234.57, GetSnapshotsRules.RoundMoney(1234.5678), Precision);

    [Fact]
    public void NewSnapshotOpensOn_WithAFreeOpenMonth_IsNull() =>
        Assert.Null(
            GetSnapshotsRules.NewSnapshotOpensOn(
                Earliest,
                Latest,
                [new SnapshotMonth(2026, CalendarMonth.August)],
                SnapshotDay
            )
        );

    [Fact]
    public void NewSnapshotOpensOn_WithAMissedMonth_IsNull() =>
        Assert.Null(
            GetSnapshotsRules.NewSnapshotOpensOn(
                Earliest,
                Latest,
                [
                    new SnapshotMonth(2026, CalendarMonth.August),
                    new SnapshotMonth(2026, CalendarMonth.October),
                ],
                SnapshotDay
            )
        );

    [Fact]
    public void NewSnapshotOpensOn_WithEveryOpenMonthTaken_IsTheSnapshotDayOfTheNextMonth() =>
        Assert.Equal(
            new DateOnly(2026, 11, 30),
            GetSnapshotsRules.NewSnapshotOpensOn(
                Earliest,
                Latest,
                [
                    new SnapshotMonth(2026, CalendarMonth.August),
                    new SnapshotMonth(2026, CalendarMonth.September),
                    new SnapshotMonth(2026, CalendarMonth.October),
                ],
                SnapshotDay
            )
        );

    [Fact]
    public void NewSnapshotOpensOn_WithAFirstSnapshotBeforeItsDay_SkipsTheTakenMonth() =>
        Assert.Equal(
            new DateOnly(2026, 11, 30),
            GetSnapshotsRules.NewSnapshotOpensOn(
                new SnapshotMonth(2026, CalendarMonth.October),
                new SnapshotMonth(2026, CalendarMonth.September),
                [new SnapshotMonth(2026, CalendarMonth.October)],
                SnapshotDay
            )
        );
}
