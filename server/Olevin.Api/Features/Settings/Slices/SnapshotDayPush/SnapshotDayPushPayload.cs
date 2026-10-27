namespace Olevin.Api.Features.Settings.Slices.SnapshotDayPush;

/// <summary>
/// The data of the push on the snapshot day; the service worker of the web app turns it into the shown text.
/// </summary>
/// <param name="Kind">The kind of the push, always <c>SnapshotDay</c>.</param>
/// <param name="Year">The year of the snapshot day.</param>
/// <param name="Month">The month of the snapshot day (1-12).</param>
/// <param name="Locale">The interface language of the user, such as <c>UK</c>.</param>
public sealed record SnapshotDayPushPayload(string Kind, int Year, int Month, string Locale);
