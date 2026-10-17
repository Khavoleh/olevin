using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.Users;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Users.Handlers;

/// <summary>
/// Creates the profile of the signed-in user.
/// </summary>
[Tags("Users")]
public static class CreateUserEndpoint
{
    /// <summary>
    /// Handles <c>POST /settings/v1/users</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The identifier of the created user (201), or a conflict (409) when the user already exists.
    /// </returns>
    [WolverinePost("/settings/v1/user", OperationId = "CreateUser")]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status409Conflict)]
    public static async Task<Response<Guid>> Post(
        ClaimsPrincipal principal,
        SettingsDbContext db,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        if (await db.Users.AnyAsync(user => user.AuthSub == authSub, ct))
            return Response<Guid>.Fail(HttpStatusCode.Conflict, "The user already exists.");

        User user = new() { Id = Guid.CreateVersion7(), AuthSub = authSub };

        db.Users.Add(user);

        return Response<Guid>.Ok(user.Id, HttpStatusCode.Created);
    }
}
