using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot.Requests;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.CreateFirstSnapshot;

/// <summary>
/// Creates the first snapshot of the signed-in user during onboarding, together with the estimates of the 2 months
/// before it.
/// </summary>
[Tags("Snapshots")]
public static class CreateFirstSnapshotEndpoint
{
    /// <summary>
    /// Handles <c>POST /financial-wellness/v1/snapshots/onboarding</c>. The snapshot is for the current month in
    /// the user's time zone.
    /// </summary>
    /// <param name="request">The first snapshot and the estimates.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus, to read the user's settings.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The identifier of the created snapshot (201), a not found (404) when the user or their settings don't exist,
    /// or a conflict (409) when the user already has snapshots.
    /// </returns>
    [WolverinePost("/financial-wellness/v1/snapshots/onboarding")]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Response<Guid>>(StatusCodes.Status409Conflict)]
    public static async Task<Response<Guid>> Post(
        CreateFirstSnapshotRequest request,
        ClaimsPrincipal principal,
        FinancialWellnessDbContext db,
        IMessageBus bus,
        TimeProvider time,
        CancellationToken ct
    )
    {
        UserSettingsDto? userSettings = await bus.InvokeAsync<UserSettingsDto?>(
            new GetUserSettingsRequest(principal.GetSubject()),
            ct
        );

        if (userSettings is null)
            return Response<Guid>.Fail(HttpStatusCode.NotFound, "The user settings do not exist.");

        bool hasSnapshots = await db.Snapshots.AnyAsync(
            snapshot => snapshot.UserId == userSettings.UserId,
            ct
        );

        if (hasSnapshots)
        {
            return Response<Guid>.Fail(
                HttpStatusCode.Conflict,
                "The first snapshot already exists."
            );
        }

        CurrencyCode baseCurrency = userSettings.BaseCurrency;
        SnapshotMonth current = SnapshotMonth.Current(time.GetUtcNow(), userSettings.Timezone);
        SnapshotMonth previous = current.Previous();
        SnapshotMonth twoMonthsAgo = previous.Previous();
        EstimateRequest previousEstimate = request.PreviousMonth;
        EstimateRequest twoMonthsAgoEstimate = request.TwoMonthsAgo;
        SnapshotAmounts amounts = CreateFirstSnapshotMapping.ToAmounts(request);
        short snapshotDay = userSettings.SnapshotDay;

        SnapshotRates currentRates = await FxRateLookup.FindAsync(
            db,
            current.DateOn(snapshotDay),
            [.. SnapshotAmountsMapping.Currencies(amounts), baseCurrency],
            ct
        );
        SnapshotRates previousRates = await FxRateLookup.FindAsync(
            db,
            previous.DateOn(snapshotDay),
            CreateFirstSnapshotMapping.Currencies(previousEstimate, baseCurrency),
            ct
        );
        SnapshotRates twoMonthsAgoRates = await FxRateLookup.FindAsync(
            db,
            twoMonthsAgo.DateOn(snapshotDay),
            CreateFirstSnapshotMapping.Currencies(twoMonthsAgoEstimate, baseCurrency),
            ct
        );

        int selfRating = request.SelfRating;
        SnapshotFigures[] history =
        [
            new(
                twoMonthsAgo,
                CreateFirstSnapshotMapping.ToFigures(
                    twoMonthsAgoEstimate,
                    twoMonthsAgoRates,
                    baseCurrency
                )
            ),
            new(
                previous,
                CreateFirstSnapshotMapping.ToFigures(previousEstimate, previousRates, baseCurrency)
            ),
            new(
                current,
                SnapshotAmountsMapping.ToFigures(
                    amounts,
                    currentRates.Rates,
                    baseCurrency,
                    selfRating
                )
            ),
        ];
        SnapshotScore score = SnapshotScorer.Score(history).Single(score => score.Month == current);

        Snapshot snapshot = SnapshotMapping.ToSnapshot(
            userSettings.UserId,
            current,
            currentRates.FxDate,
            amounts,
            selfRating
        );

        SnapshotMapping.ApplyScore(snapshot, score);

        db.EstimatedSnapshots.AddRange(
            CreateFirstSnapshotMapping.ToEstimatedSnapshot(
                userSettings.UserId,
                twoMonthsAgo,
                twoMonthsAgoEstimate,
                twoMonthsAgoRates
            ),
            CreateFirstSnapshotMapping.ToEstimatedSnapshot(
                userSettings.UserId,
                previous,
                previousEstimate,
                previousRates
            )
        );
        db.Snapshots.Add(snapshot);

        return Response<Guid>.Ok(snapshot.Id, HttpStatusCode.Created);
    }
}
