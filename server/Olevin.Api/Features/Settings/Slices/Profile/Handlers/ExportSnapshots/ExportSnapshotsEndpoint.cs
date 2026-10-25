using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots.DTOs;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.ExportSnapshots;

/// <summary>
/// Gives every snapshot of the signed-in user as a file.
/// </summary>
[Tags("Profile")]
public static class ExportSnapshotsEndpoint
{
    /// <summary>
    /// Handles <c>GET /settings/v1/user-settings/snapshots/export?format=Json|Csv</c>. The format is JSON when omitted.
    /// </summary>
    /// <param name="format">The format of the file.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="bus">The message bus, to read the snapshots in the financial wellness feature.</param>
    /// <param name="json">The JSON options of the API.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The file with every snapshot, the oldest first, empty without snapshots (200), or a not found (404) when the
    /// user or their settings don't exist.
    /// </returns>
    [WolverineGet("/settings/v1/user-settings/snapshots/export", OperationId = "ExportSnapshots")]
    [Produces("application/json", "text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<IResult> Get(
        ExportFormat format,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        IMessageBus bus,
        IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions> json,
        TimeProvider time,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        Guid? userId = await db
            .UserSettings.Where(setting => setting.User.AuthSub == authSub)
            .Select(setting => (Guid?)setting.UserId)
            .FirstOrDefaultAsync(ct);

        if (userId is null)
        {
            return TypedResults.Json(
                Response<Guid>.Fail(HttpStatusCode.NotFound, "The settings do not exist."),
                statusCode: StatusCodes.Status404NotFound
            );
        }

        IReadOnlyList<UserSnapshotDto> snapshots = await bus.InvokeAsync<
            IReadOnlyList<UserSnapshotDto>
        >(new GetAllSnapshotsRequest(userId.Value), ct);

        byte[] file =
            format == ExportFormat.Csv
                ? ExportSnapshotsRules.Csv(snapshots)
                : ExportSnapshotsRules.Json(snapshots, json.Value.SerializerOptions);

        return TypedResults.File(
            file,
            ExportSnapshotsRules.ContentType(format),
            ExportSnapshotsRules.FileName(
                format,
                DateOnly.FromDateTime(time.GetUtcNow().UtcDateTime)
            )
        );
    }
}
