using System.Text.Json;
using System.Text.Json.Serialization;

namespace Olevin.Api.Features.Settings.Slices.Users.Handlers;

/// <summary>
/// The body of a Logto webhook request about a change made through the Management API.
/// </summary>
/// <param name="Event">The name of the event, such as <c>User.Deleted</c>.</param>
/// <param name="Params">The route parameters of the Management API call.</param>
public sealed record UserDeletedPayload(
    [property: JsonPropertyName("event")] string? Event,
    [property: JsonPropertyName("params")] UserDeletedParams? Params
)
{
    private const string UserDeletedEvent = "User.Deleted";

    /// <summary>
    /// Gets the identifier of the deleted user, or <see langword="null"/> for any other event or a missing identifier.
    /// </summary>
    [JsonIgnore]
    public string? DeletedUserId =>
        Event == UserDeletedEvent && !string.IsNullOrWhiteSpace(Params?.UserId)
            ? Params.UserId
            : null;

    /// <summary>
    /// Reads the payload from the raw body of a request.
    /// </summary>
    /// <param name="body">The raw body.</param>
    /// <returns>The payload, or <see langword="null"/> when the body is not a valid payload.</returns>
    public static UserDeletedPayload? Parse(ReadOnlySpan<byte> body)
    {
        try
        {
            return JsonSerializer.Deserialize<UserDeletedPayload>(body);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}

/// <summary>
/// The route parameters of the Management API call that Logto reports.
/// </summary>
/// <param name="UserId">The identifier of the user in the route.</param>
public sealed record UserDeletedParams([property: JsonPropertyName("userId")] string? UserId);
