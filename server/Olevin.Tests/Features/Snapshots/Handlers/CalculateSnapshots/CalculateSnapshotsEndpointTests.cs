using Olevin.Api.Features.Snapshots.Currency;
using Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;
using Olevin.Api.Shared.Index;
using Olevin.Api.Shared.Index.Model;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// <c>CalculateSnapshotsEndpoint.Post</c> called directly, without the web host.
/// </summary>
public sealed class CalculateSnapshotsEndpointTests
{
    private const double Precision = 0.1;

    [Fact]
    public void ControlExample_InMixedCurrencies_GivesTheReferenceResults()
    {
        CalculateSnapshotsResponse response = CalculateSnapshotsEndpoint.Post(
            ControlExample.Request()
        );

        ScoreResponse september = response.Snapshots[^1].Score!;

        Assert.Equal(new DateOnly(2026, 9, 1), response.Snapshots[^1].Month);
        Assert.Equal(3.5, september.Inputs.Indicators.Reserve, 1e-9);
        Assert.Equal(50.0, september.CashFlow, Precision);
        Assert.Equal(60.4, september.Buffer, Precision);
        Assert.Equal(56.2, september.FuzzyIndex, Precision);
        Assert.Equal(49.8, september.LinearIndex, Precision);
        Assert.Equal(58.7, september.Index, Precision);
        Assert.Equal(Zone.Unstable, september.Zone);
        Assert.Equal(Zone.Unstable, september.FuzzyZone);
        Assert.Equal(Zone.Unstable, september.LinearZone);
        Assert.False(september.IsPreliminary);
        Assert.Equal(IndexConstants.ModelVersion, response.ModelVersion);
    }

    [Fact]
    public void Snapshots_AreSortedByMonth_AndEstimatesHaveNoScore()
    {
        CalculateSnapshotsRequest request = ControlExample.Request();
        CalculateSnapshotsRequest shuffled = request with
        {
            Snapshots = [.. request.Snapshots.Reverse()],
        };

        CalculateSnapshotsResponse response = CalculateSnapshotsEndpoint.Post(shuffled);

        Assert.Equal(
            [.. request.Snapshots.Select(s => s.Month)],
            response.Snapshots.Select(s => s.Month)
        );
        Assert.Null(response.Snapshots[0].Score);
        Assert.Null(response.Snapshots[1].Score);
        Assert.Equal(56.2, response.Snapshots[^1].Score!.FuzzyIndex, Precision);
    }

    [Fact]
    public void BaseCurrency_ChangesTheAmountsButNotTheIndex()
    {
        CalculateSnapshotsRequest request = ControlExample.Request();
        CalculateSnapshotsRequest inUsd = request with
        {
            BaseCurrency = CurrencyCode.USD,
            Snapshots =
            [
                .. request.Snapshots.Select(s => s with { FxRates = ControlExample.UsdRates }),
            ],
        };

        ScoreResponse uah = CalculateSnapshotsEndpoint.Post(request).Snapshots[^1].Score!;
        ScoreResponse usd = CalculateSnapshotsEndpoint.Post(inUsd).Snapshots[^1].Score!;

        Assert.Equal(uah.FuzzyIndex, usd.FuzzyIndex, 1e-9);
        Assert.Equal(36_000 / ControlExample.UsdRate, usd.Inputs.AverageExpenses, 1e-9);
    }
}
