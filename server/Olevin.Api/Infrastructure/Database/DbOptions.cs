namespace Olevin.Api.Infrastructure.Database;

/// <summary>
/// Connection strings of the databases this API uses.
/// </summary>
public sealed class DbOptions
{
    /// <summary>
    /// The configuration section that holds these settings.
    /// </summary>
    public const string SectionName = "ConnectionStrings";

    /// <summary>
    /// Gets the connection string of the Olevin database.
    /// </summary>
    public required string Olevin { get; init; }
}
