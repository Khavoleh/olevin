using System.Net;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;

/// <summary>
/// The snapshot rules for the month.
/// </summary>
public static class CreateSnapshotRules
{
    /// <summary>
    /// Finds the earliest month a snapshot may be created for.
    /// </summary>
    /// <param name="estimates">The months of the stored estimates.</param>
    /// <returns>The month after the latest estimate.</returns>
    public static SnapshotMonth EarliestMonth(IEnumerable<SnapshotMonth> estimates) =>
        estimates.MaxBy(estimate => (estimate.Year, estimate.Month))!.Next();

    /// <summary>
    /// Checks that a snapshot may be created for the month.
    /// </summary>
    /// <param name="month">The month of the new snapshot.</param>
    /// <param name="earliestMonth">The earliest allowed month.</param>
    /// <param name="currentMonth">The current month in the user's time zone.</param>
    /// <param name="taken">The months of the stored snapshots.</param>
    /// <returns>The failed response, or <see langword="null"/> when the month is allowed.</returns>
    public static Response<Guid>? RejectMonth(
        SnapshotMonth month,
        SnapshotMonth earliestMonth,
        SnapshotMonth currentMonth,
        IEnumerable<SnapshotMonth> taken
    ) =>
        month switch
        {
            _ when Compare(month, earliestMonth) < 0 => Response<Guid>.Fail(
                HttpStatusCode.BadRequest,
                "The snapshot cannot be for an onboarding month or earlier."
            ),
            _ when Compare(month, currentMonth) > 0 => Response<Guid>.Fail(
                HttpStatusCode.BadRequest,
                "The snapshot cannot be for a month that has not started yet."
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
