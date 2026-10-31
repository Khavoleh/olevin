using System.Net;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;

/// <summary>
/// The snapshot rules for the month.
/// </summary>
public static class CreateSnapshotRules
{
    /// <summary>
    /// Checks that a snapshot may be created for the month.
    /// </summary>
    /// <param name="month">The month of the new snapshot.</param>
    /// <param name="earliestMonth">The earliest allowed month.</param>
    /// <param name="latestMonth">The latest month whose snapshot day has come.</param>
    /// <param name="taken">The months of the stored snapshots.</param>
    /// <returns>The failed response, or <see langword="null"/> when the month is allowed.</returns>
    public static Response<Guid>? RejectMonth(
        SnapshotMonth month,
        SnapshotMonth earliestMonth,
        SnapshotMonth latestMonth,
        IEnumerable<SnapshotMonth> taken
    ) =>
        month switch
        {
            _ when Compare(month, earliestMonth) < 0 => Response<Guid>.Fail(
                HttpStatusCode.BadRequest,
                "The snapshot cannot be for an onboarding month or earlier."
            ),
            _ when Compare(month, latestMonth) > 0 => Response<Guid>.Fail(
                HttpStatusCode.BadRequest,
                "The snapshot day of this month has not come yet."
            ),
            _ when taken.Contains(month) => Response<Guid>.Fail(
                HttpStatusCode.Conflict,
                "The snapshot of this month already exists."
            ),
            _ => null,
        };

    private static int Compare(SnapshotMonth left, SnapshotMonth right) =>
        (left.Year, left.Month).CompareTo((right.Year, right.Month));
}
