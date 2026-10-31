namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateSnapshotDay;

/// <summary>
/// The new snapshot day of a user.
/// </summary>
/// <param name="SnapshotDay">The day of the month (1-31) the monthly snapshot is taken on.</param>
public sealed record UpdateSnapshotDayRequest(short SnapshotDay);
