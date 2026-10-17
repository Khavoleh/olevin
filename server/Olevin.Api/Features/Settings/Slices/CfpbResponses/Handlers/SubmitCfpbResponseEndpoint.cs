using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.CfpbResponses;
using Olevin.Api.Features.Settings.Data.Users;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.CfpbResponses.Handlers;

/// <summary>
/// Saves the CFPB Financial Well-Being Scale answers of the signed-in user during onboarding, once.
/// </summary>
[Tags("CfpbResponses")]
public static class SubmitCfpbResponseEndpoint
{
    /// <summary>
    /// Handles <c>POST /settings/v1/cfpb-response</c>.
    /// </summary>
    /// <param name="request">The answers of the user.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The identifier of the created response (201), a forbidden (403) when the user has not consented to research,
    /// a not found (404) when the user or their settings don't exist,
    /// or a conflict (409) when the user has already answered.
    /// </returns>
    [WolverinePost("/settings/v1/cfpb-response", OperationId = "SubmitCfpbResponse")]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status409Conflict)]
    public static async Task<Response<Guid>> Post(
        SubmitCfpbResponseRequest request,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        User? user = await db
            .Users.Include(user => user.Setting)
            .Include(user => user.CfpbResponse)
            .FirstOrDefaultAsync(user => user.AuthSub == authSub, ct);

        if (user is null)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The user does not exist.");

        if (user.Setting is null)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The user settings do not exist.");

        if (!user.Setting.ResearchConsent)
        {
            return Response<Guid>.Fail(
                HttpStatusCode.Forbidden,
                "The user has not consented to research."
            );
        }

        if (user.CfpbResponse is not null)
        {
            return Response<Guid>.Fail(
                HttpStatusCode.Conflict,
                "The CFPB response already exists."
            );
        }

        short[] answers = SubmitCfpbResponseMapping.ToAnswers(request);

        CfpbResponse response = new()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Answers = answers,
            Is62Plus = request.Is62Plus,
            Score = CfpbScoreTableHelper.ToScore(answers.Sum(answer => answer), request.Is62Plus),
        };

        user.CfpbResponse = response;

        return Response<Guid>.Ok(response.Id, HttpStatusCode.Created);
    }
}
