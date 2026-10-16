using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
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
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The snapshot identifier (201), a bad request (400) for a month that is not allowed, a not found (404) without
    /// user settings, or a conflict (409) without onboarding or for a taken month.
    /// </returns>
    [WolverinePost("/financial-wellness/v1/snapshots")]
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
        SnapshotMonth[] taken = [.. snapshots.Select(CreateSnapshotMapping.MonthOf)];

        Response<Guid>? rejection = CreateSnapshotRules.RejectMonth(
            month,
            CreateSnapshotRules.EarliestMonth(estimates.Select(CreateSnapshotMapping.MonthOf)),
            SnapshotMonth.Current(now, userSettings.Timezone),
            taken
        );

        if (rejection is not null)
            return rejection;

        CurrencyCode baseCurrency = userSettings.BaseCurrency;
        SnapshotAmounts amounts = CreateSnapshotMapping.ToAmounts(request);
        int selfRating = request.SelfRating;

        SnapshotRates rates = await FxRateLookup.FindAsync(
            db,
            month.DateOn(userSettings.SnapshotDay),
            [.. SnapshotAmountsMapping.Currencies(amounts), baseCurrency],
            ct
        );
        Dictionary<DateOnly, ExchangeRates> storedRates = await FxRateLookup.LoadAsync(
            db,
            [
                .. snapshots.Select(snapshot => snapshot.FxDate),
                .. estimates.Select(estimate => estimate.FxDate),
            ],
            ct
        );

        SnapshotFigures[] history =
        [
            .. estimates.Select(estimate =>
                CreateSnapshotMapping.ToFigures(
                    estimate,
                    storedRates[estimate.FxDate],
                    baseCurrency
                )
            ),
            .. snapshots.Select(snapshot =>
                CreateSnapshotMapping.ToFigures(
                    snapshot,
                    storedRates[snapshot.FxDate],
                    baseCurrency
                )
            ),
            new(
                month,
                SnapshotAmountsMapping.ToFigures(amounts, rates.Rates, baseCurrency, selfRating)
            ),
        ];
        Dictionary<SnapshotMonth, SnapshotScore> scores = SnapshotScorer
            .Score([.. history.OrderBy(figures => (figures.Month.Year, figures.Month.Month))])
            .ToDictionary(score => score.Month);

        foreach (
            Snapshot later in snapshots.Where(stored =>
                (stored.Year, stored.Month).CompareTo((month.Year, month.Month)) > 0
            )
        )
        {
            SnapshotMapping.ApplyScore(later, scores[CreateSnapshotMapping.MonthOf(later)]);
            later.UpdatedAt = now.UtcDateTime;
        }

        Snapshot snapshot = SnapshotMapping.ToSnapshot(
            userId,
            month,
            rates.FxDate,
            amounts,
            selfRating,
            scores[month]
        );

        db.Snapshots.Add(snapshot);

        return Response<Guid>.Ok(snapshot.Id, HttpStatusCode.Created);
    }
}
