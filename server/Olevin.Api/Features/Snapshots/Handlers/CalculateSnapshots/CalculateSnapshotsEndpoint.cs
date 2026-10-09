using Olevin.Api.Shared.Index;
using Olevin.Api.Shared.Index.Indicators;
using Wolverine.Http;

namespace Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;

/// <summary>
/// Calculates the indexes, advice and changes for a series of monthly snapshots without storing them.
/// </summary>
[Tags("Snapshots")]
public static class CalculateSnapshotsEndpoint
{
    /// <summary>
    /// Handles <c>POST /snapshots/v1/calculate</c>.
    /// </summary>
    /// <param name="request">The snapshots.</param>
    /// <returns>The score of every actual snapshot, in month order.</returns>
    [WolverinePost("/snapshots/v1/calculate")]
    public static CalculateSnapshotsResponse Post(CalculateSnapshotsRequest request)
    {
        SnapshotRequest[] snapshots = [.. request.Snapshots.OrderBy(snapshot => snapshot.Month)];
        MonthFigures[] months =
        [
            .. snapshots.Select(snapshot =>
                CalculateSnapshotsMapping.ToFigures(snapshot, request.BaseCurrency)
            ),
        ];
        IReadOnlyList<SnapshotScore?> scores = SnapshotScorer.Score(months);

        SnapshotResponse[] results =
        [
            .. snapshots.Select(
                (snapshot, i) =>
                    new SnapshotResponse(
                        new DateOnly(snapshot.Month.Year, snapshot.Month.Month, 1),
                        snapshot.IsEstimated,
                        scores[i] is { } score ? CalculateSnapshotsMapping.ToResponse(score) : null
                    )
            ),
        ];

        return new CalculateSnapshotsResponse(
            IndexConstants.ModelVersion,
            request.BaseCurrency,
            results
        );
    }
}
