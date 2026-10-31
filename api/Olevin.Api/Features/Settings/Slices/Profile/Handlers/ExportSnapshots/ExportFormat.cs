namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.ExportSnapshots;

/// <summary>
/// The format of the exported snapshots.
/// </summary>
public enum ExportFormat
{
    /// <summary>
    /// A JSON array with a snapshot per item.
    /// </summary>
    Json,

    /// <summary>
    /// A CSV table with a snapshot per row.
    /// </summary>
    Csv,
}
