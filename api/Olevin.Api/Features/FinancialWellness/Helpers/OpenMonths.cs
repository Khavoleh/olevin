namespace Olevin.Api.Features.FinancialWellness.Helpers;

/// <summary>
/// The range of months a snapshot may be created for.
/// </summary>
public static class OpenMonths
{
    /// <summary>
    /// Finds the earliest month a snapshot may be created for.
    /// </summary>
    /// <param name="estimates">The months of the stored estimates.</param>
    /// <returns>The month after the latest estimate.</returns>
    public static SnapshotMonth Earliest(IEnumerable<SnapshotMonth> estimates) =>
        estimates.MaxBy(estimate => (estimate.Year, estimate.Month))!.Next();

    /// <summary>
    /// Finds the latest month whose snapshot day has come.
    /// </summary>
    /// <param name="today">The current date in the user's time zone.</param>
    /// <param name="snapshotDay">The day of the month (1-31) from the user's settings.</param>
    /// <returns>The current month from its snapshot day, the previous month before it.</returns>
    public static SnapshotMonth Latest(DateOnly today, short snapshotDay)
    {
        SnapshotMonth current = SnapshotMonth.Of(today);

        return today < current.DateOn(snapshotDay) ? current.Previous() : current;
    }

    /// <summary>
    /// Lists the months a new snapshot may be created for now.
    /// </summary>
    /// <param name="earliest">The earliest month a snapshot may be created for.</param>
    /// <param name="latest">The latest month whose snapshot day has come.</param>
    /// <param name="taken">The months of the stored snapshots.</param>
    /// <returns>The months without a snapshot, the latest first.</returns>
    public static IReadOnlyList<SnapshotMonth> Free(
        SnapshotMonth earliest,
        SnapshotMonth latest,
        IReadOnlyCollection<SnapshotMonth> taken
    )
    {
        List<SnapshotMonth> free = [];

        for (
            SnapshotMonth month = latest;
            (month.Year, month.Month).CompareTo((earliest.Year, earliest.Month)) >= 0;
            month = month.Previous()
        )
        {
            if (!taken.Contains(month))
                free.Add(month);
        }

        return free;
    }

    /// <summary>
    /// Finds when a new snapshot can be created.
    /// </summary>
    /// <param name="earliest">The earliest month a snapshot may be created for.</param>
    /// <param name="latest">The latest month whose snapshot day has come.</param>
    /// <param name="taken">The months of the stored snapshots.</param>
    /// <param name="snapshotDay">The day of the month (1-31) from the user's settings.</param>
    /// <returns>
    /// The snapshot day of the first free month, or <see langword="null"/> when that month is already open.
    /// </returns>
    public static DateOnly? OpensOn(
        SnapshotMonth earliest,
        SnapshotMonth latest,
        IReadOnlyCollection<SnapshotMonth> taken,
        short snapshotDay
    )
    {
        SnapshotMonth month = earliest;

        while (taken.Contains(month))
            month = month.Next();

        return (month.Year, month.Month).CompareTo((latest.Year, latest.Month)) <= 0
            ? null
            : month.DateOn(snapshotDay);
    }
}
