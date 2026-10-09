using System.Net;
using System.Reflection;
using Wolverine.Http;

namespace Olevin.Api.Shared;

/// <summary>
/// The envelope every endpoint returns. The HTTP status is the same as <paramref name="Code"/>.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
/// <param name="Code">The result code, an HTTP status code.</param>
/// <param name="Error">The reason of the failure, or <see langword="null"/> on success.</param>
/// <param name="Data">The result, or <see langword="null"/> on failure.</param>
public sealed record Response<T>(HttpStatusCode Code, string? Error, T? Data) : IHttpAware
{
    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="data">The result.</param>
    /// <param name="code">The result code.</param>
    /// <returns>The response.</returns>
    public static Response<T> Ok(T data, HttpStatusCode code = HttpStatusCode.OK) =>
        new(code, null, data);

    /// <summary>
    /// Creates a failed response.
    /// </summary>
    /// <param name="code">The result code.</param>
    /// <param name="error">The reason of the failure.</param>
    /// <returns>The response.</returns>
    public static Response<T> Fail(HttpStatusCode code, string error) => new(code, error, default);

    /// <inheritdoc />
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder) { }

    /// <inheritdoc />
    public void Apply(HttpContext context) => context.Response.StatusCode = (int)Code;
}
