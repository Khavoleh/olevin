using System.Net;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;
using Olevin.Api.Shared;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;

public sealed class CreateSnapshotRulesTests
{
    private static readonly SnapshotMonth Earliest = new(2026, CalendarMonth.June);
    private static readonly SnapshotMonth Current = new(2026, CalendarMonth.October);
    private static readonly SnapshotMonth[] Taken =
    [
        new(2026, CalendarMonth.July),
        new(2026, CalendarMonth.October),
    ];

    private static Response<Guid>? Reject(SnapshotMonth month) =>
        CreateSnapshotRules.RejectMonth(month, Earliest, Current, Taken);

    [Theory]
    [InlineData(2026, CalendarMonth.June)]
    [InlineData(2026, CalendarMonth.August)]
    [InlineData(2026, CalendarMonth.September)]
    public void RejectMonth_FreeMonthFromTheEarliestToNow_IsAllowed(
        short year,
        CalendarMonth month
    ) => Assert.Null(Reject(new SnapshotMonth(year, month)));

    [Theory]
    [InlineData(2026, CalendarMonth.May)]
    [InlineData(2025, CalendarMonth.December)]
    public void RejectMonth_BeforeTheEarliest_IsBadRequest(short year, CalendarMonth month) =>
        Assert.Equal(HttpStatusCode.BadRequest, Reject(new SnapshotMonth(year, month))?.Code);

    [Theory]
    [InlineData(2026, CalendarMonth.November)]
    [InlineData(2027, CalendarMonth.January)]
    public void RejectMonth_NotStartedYet_IsBadRequest(short year, CalendarMonth month) =>
        Assert.Equal(HttpStatusCode.BadRequest, Reject(new SnapshotMonth(year, month))?.Code);

    [Theory]
    [InlineData(2026, CalendarMonth.July)]
    [InlineData(2026, CalendarMonth.October)]
    public void RejectMonth_AlreadyTaken_IsConflict(short year, CalendarMonth month) =>
        Assert.Equal(HttpStatusCode.Conflict, Reject(new SnapshotMonth(year, month))?.Code);

    [Fact]
    public void EarliestMonth_IsTheMonthAfterTheLatestEstimate() =>
        Assert.Equal(
            new SnapshotMonth(2026, CalendarMonth.January),
            CreateSnapshotRules.EarliestMonth([
                new SnapshotMonth(2025, CalendarMonth.December),
                new SnapshotMonth(2025, CalendarMonth.November),
            ])
        );
}
