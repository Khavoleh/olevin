using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Snapshots.Mappings;

public sealed class SnapshotMappingTests
{
    private static readonly SnapshotAmounts Amounts = ControlExample.SeptemberAmounts();

    private static Snapshot Map() =>
        SnapshotMapping.ToSnapshot(
            Guid.CreateVersion7(),
            new SnapshotMonth(2026, CalendarMonth.September),
            new DateOnly(2026, 9, 25),
            Amounts,
            4
        );

    [Fact]
    public void Month_RatesAndAmounts_AreKept()
    {
        Snapshot snapshot = Map();

        Assert.Equal(CalendarMonth.September, snapshot.Month);
        Assert.Equal(2026, snapshot.Year);
        Assert.Equal(new DateOnly(2026, 9, 25), snapshot.FxDate);
        Assert.Equal(4, snapshot.SelfRating);
        Assert.Same(Amounts, snapshot.Amounts);
    }

    [Fact]
    public void EveryMapping_GetsANewId() => Assert.NotEqual(Map().Id, Map().Id);
}
