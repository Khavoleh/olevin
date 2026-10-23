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
    /// Gets the connection string of the role that owns Wolverine's <c>outbox</c> schema.
    /// </summary>
    public required string Wolverine { get; init; }

    /// <summary>
    /// Gets the connection string of the role that reads and writes the <c>settings</c> schema.
    /// </summary>
    public required string Settings { get; init; }

    /// <summary>
    /// Gets the connection string of the role that reads and writes the <c>financial_wellness</c> schema.
    /// </summary>
    public required string FinancialWellness { get; init; }
}
