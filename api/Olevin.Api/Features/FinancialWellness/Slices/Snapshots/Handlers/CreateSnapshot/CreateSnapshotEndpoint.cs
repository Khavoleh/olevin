using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Scoring;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateSnapshot;

/// <summary>
/// Creates a snapshot of the signed-in user after the onboarding.
/// </summary>
[Tags("Snapshots")]
public static class CreateSnapshotEndpoint
{
    /// <summary>
    /// Handles <c>POST /financial-wellness/v1/snapshots</c>. Later snapshots are scored again.
    /// </summary>
    /// <param name="request">The snapshot.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The snapshot identifier (201), a bad request (400) for a month that is not allowed or whose snapshot day has
    /// not come yet, a not found (404) without user settings, or a conflict (409) without onboarding or for a taken
    /// month.
    /// </returns>
    [WolverinePost("/financial-wellness/v1/snapshots", OperationId = "CreateSnapshot")]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status409Conflict)]
    public static async Task<Response<Guid>> Post(
        CreateSnapshotRequest request,
        ClaimsPrincipal principal,
        FinancialWellnessDbContext db,
        IMessageBus bus,
        TimeProvider time,
        ILogger logger,
        CancellationToken ct
    )
    {
        UserSettingsDto? userSettings = await bus.InvokeAsync<UserSettingsDto?>(
            new GetUserSettingsRequest(principal.GetSubject()),
            ct
        );

        if (userSettings is null)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The user settings do not exist.");

        Guid userId = userSettings.UserId;
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userId)
            .ToListAsync(ct);

        if (snapshots.Count == 0)
        {
            return Response<Guid>.Fail(
                HttpStatusCode.Conflict,
                "The first snapshot does not exist."
            );
        }

        List<EstimatedSnapshot> estimates = await db
            .EstimatedSnapshots.Where(estimate => estimate.UserId == userId)
            .ToListAsync(ct);

        DateTimeOffset now = time.GetUtcNow();
        SnapshotMonth month = new(request.Year, request.Month);
        SnapshotMonth[] taken = [.. snapshots.Select(SnapshotHistoryMapping.MonthOf)];

        Response<Guid>? rejection = CreateSnapshotRules.RejectMonth(
            month,
            OpenMonths.Earliest(estimates.Select(SnapshotHistoryMapping.MonthOf)),
            OpenMonths.Latest(
                SnapshotMonth.Today(now, userSettings.Timezone),
                userSettings.SnapshotDay
            ),
            taken
        );

        if (rejection is not null)
            return rejection;

        CurrencyCode baseCurrency = userSettings.BaseCurrency;
        SnapshotAmounts amounts = SnapshotAmountsMapping.ToAmounts(
            request.Income,
            request.Expenses,
            request.Saved,
            request.Reserves,
            request.DebtPayments
        );

        SnapshotRates rates = await FxRateLookup.FindAsync(
            db,
            month.DateOn(userSettings.SnapshotDay),
            [.. SnapshotAmountsMapping.Currencies(amounts), baseCurrency],
            ct
        );

        Snapshot snapshot = SnapshotMapping.ToSnapshot(
            userId,
            month,
            rates.FxDate,
            amounts,
            request.SelfRating
        );
        Dictionary<SnapshotMonth, SnapshotScore> scores = await SnapshotRescorer.ScoreAsync(
            db,
            estimates,
            [.. snapshots, snapshot],
            baseCurrency,
            ct
        );

        SnapshotRescorer.ApplyFrom(snapshots, month, scores, now.UtcDateTime);
        SnapshotScoreMapping.ApplyScore(snapshot, scores[month]);

        db.Snapshots.Add(snapshot);

        logger.LogInformation(
            "Created the snapshot {SnapshotId} of {Month} for the user {UserId}.",
            snapshot.Id,
            month,
            userId
        );

        return Response<Guid>.Ok(snapshot.Id, HttpStatusCode.Created);
    }
}
