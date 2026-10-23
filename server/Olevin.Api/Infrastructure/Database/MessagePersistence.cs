using JasperFx;
using Wolverine;
using Wolverine.Postgresql;

namespace Olevin.Api.Infrastructure.Database;

/// <summary>
/// Stores the Wolverine inbox and outbox in the database, so that a message sent by a handler is saved in the same
/// transaction as its data and is never lost or sent for a rolled back change.
/// </summary>
public static class MessagePersistence
{
    /// <summary>
    /// The schema of the Wolverine tables.
    /// </summary>
    public const string Schema = "outbox";

    /// <summary>
    /// Stores the messages in PostgreSQL and makes the local queues durable, so that the transactional middleware
    /// writes every sent message to the outbox before the commit.
    /// </summary>
    /// <param name="options">The Wolverine options.</param>
    /// <param name="configuration">The application configuration with the <c>Wolverine</c> connection string.</param>
    /// <returns>The same options for chaining.</returns>
    /// <exception cref="InvalidOperationException">The connection string is missing.</exception>
    public static WolverineOptions PersistMessagesInDatabase(
        this WolverineOptions options,
        IConfiguration configuration
    )
    {
        string connectionString =
            configuration.GetConnectionString(nameof(DbOptions.Wolverine))
            ?? throw new InvalidOperationException("The Wolverine connection string is missing.");

        options.PersistMessagesWithPostgresql(connectionString, Schema);
        options.AutoBuildMessageStorageOnStartup = AutoCreate.CreateOrUpdate;
        options.Policies.UseDurableLocalQueues();

        return options;
    }
}
