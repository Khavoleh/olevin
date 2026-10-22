using System.Net;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Requests;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Shared;
using Wolverine;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.CreateFirstSnapshot;

/// <summary>
/// Saves the first snapshot of a user during onboarding, together with the estimates of the 2 months before it.
/// </summary>
public static class CreateFirstSnapshotHandler
{
    /// <summary>
    /// Handles <see cref="CreateFirstSnapshotRequest"/>. The snapshot is for the current month in the user's time zone.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus, to read the user's settings.</param>
    /// <param name="time">The clock.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The identifier of the created snapshot (201), a not found (404) when the user or their settings don't exist,
    /// or a conflict (409) when the user already has snapshots.
    /// </returns>
    public static async Task<Response<Guid>> Handle(
        CreateFirstSnapshotRequest request,
        FinancialWellnessDbContext db,
        IMessageBus bus,
        TimeProvider time,
        CancellationToken ct
    )
    {
        UserSettingsDto? userSettings = await bus.InvokeAsync<UserSettingsDto?>(
            new GetUserSettingsRequest(request.AuthSub),
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

        FirstSnapshot first = request.Snapshot;
        CurrencyCode baseCurrency = userSettings.BaseCurrency;
        SnapshotMonth current = SnapshotMonth.Current(time.GetUtcNow(), userSettings.Timezone);
        SnapshotMonth previous = current.Previous();
        SnapshotMonth twoMonthsAgo = previous.Previous();
        EstimateRequest previousEstimate = first.PreviousMonth;
        EstimateRequest twoMonthsAgoEstimate = first.TwoMonthsAgo;
        SnapshotAmounts amounts = CreateFirstSnapshotMapping.ToAmounts(first);
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

        int selfRating = first.SelfRating;
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
                SnapshotHistoryMapping.ToFigures(
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
