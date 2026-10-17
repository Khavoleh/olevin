using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using Wolverine.Http;

namespace Olevin.Api.Shared;

/// <summary>
/// The envelope every endpoint returns.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
/// <param name="Code">The HTTP status code of the response, not a part of the body.</param>
/// <param name="Error">The reason of the failure, or <see langword="null"/> on success.</param>
/// <param name="Data">The result, or the default value of <typeparamref name="T"/> on failure.</param>
/// <param name="ValidationErrors">The failed validation rules, or <see langword="null"/> when the request is valid.</param>
public sealed record Response<T>(
    [property: JsonIgnore] HttpStatusCode Code,
    string? Error,
    T? Data,
    IReadOnlyList<string>? ValidationErrors
) : IHttpAware
{
    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="data">The result.</param>
    /// <param name="code">The result code.</param>
    /// <returns>The response.</returns>
    public static Response<T> Ok(T data, HttpStatusCode code = HttpStatusCode.OK) =>
        new(code, null, data, null);

    /// <summary>
    /// Creates a failed response.
    /// </summary>
    /// <param name="code">The result code.</param>
    /// <param name="error">The reason of the failure.</param>
    /// <returns>The response.</returns>
    public static Response<T> Fail(HttpStatusCode code, string error) =>
        new(code, error, default, null);

    /// <summary>
    /// Creates a bad request response for a request that failed validation.
    /// </summary>
    /// <param name="errors">The messages of the failed validation rules.</param>
    /// <returns>The response.</returns>
    public static Response<T> Invalid(IEnumerable<string> errors) =>
        new(HttpStatusCode.BadRequest, "The request is not valid.", default, [.. errors]);

    /// <inheritdoc />
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder) { }

    /// <inheritdoc />
    public void Apply(HttpContext context) => context.Response.StatusCode = (int)Code;
}
