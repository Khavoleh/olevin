using System.Security.Claims;
using Olevin.Api.Infrastructure.Auth;
using Wolverine.Http;

namespace Olevin.Api.Features.Account;

/// <summary>
/// Returns the signed-in user as identified by the Logto access token.
/// </summary>
public static class GetMe
{
    /// <summary>
    /// Handles <c>GET /me</c>.
    /// </summary>
    /// <param name="user">The user from the validated access token.</param>
    /// <returns>The Logto identifier of the user.</returns>
    [WolverineGet("/me")]
    public static Response Handle(ClaimsPrincipal user)
    {
        return new Response(user.GetSubject());
    }

    /// <summary>
    /// The signed-in user.
    /// </summary>
    /// <param name="Subject">The Logto user identifier (<c>sub</c>).</param>
    public sealed record Response(string Subject);
}
