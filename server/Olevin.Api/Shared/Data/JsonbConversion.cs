using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Olevin.Api.Shared.Data;

/// <summary>
/// Stores a value as a <c>jsonb</c> document with snake_case keys and enums as their names. The document is always
/// read and written whole and never queried, so plain serialization is enough and keeps the domain records as they are.
/// </summary>
public static class JsonbConversion
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Maps the property to a <c>jsonb</c> column. Values are compared by their JSON, so a changed list inside a
    /// record is noticed too.
    /// </summary>
    /// <typeparam name="T">The type of the stored value.</typeparam>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder for chaining.</returns>
    public static PropertyBuilder<T> HasJsonbConversion<T>(this PropertyBuilder<T> builder)
        where T : class?
    {
        return builder
            .HasColumnType("jsonb")
            .HasConversion(
                value => Serialize(value),
                json => Deserialize<T>(json),
                new ValueComparer<T>(
                    (left, right) => Serialize(left) == Serialize(right),
                    value => Serialize(value).GetHashCode(StringComparison.Ordinal),
                    value => Deserialize<T>(Serialize(value))
                )
            );
    }

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    private static T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, Options)
        ?? throw new JsonException($"The stored {typeof(T).Name} is null.");
}
