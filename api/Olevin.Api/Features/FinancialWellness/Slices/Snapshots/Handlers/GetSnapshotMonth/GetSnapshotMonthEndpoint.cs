using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshotMonth.Responses;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetSnapshotMonth;

/// <summary>
/// Gives the amounts of one month of the signed-in user, so they can be changed.
/// </summary>
[Tags("Snapshots")]
public static class GetSnapshotMonthEndpoint
{
    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/snapshots/months/{year}/{month}</c>.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The financial wellness database.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The snapshot or the estimate of the month (200), or a not found (404) without user settings or when the month
    /// has neither.
    /// </returns>
    [WolverineGet(
        "/financial-wellness/v1/snapshots/months/{year}/{month}",
        OperationId = "GetSnapshotMonth"
    )]
    [ProducesResponseType<Response<GetSnapshotMonthResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<GetSnapshotMonthResponse>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<GetSnapshotMonthResponse>> Get(
        short year,
        CalendarMonth month,
        ClaimsPrincipal principal,
        FinancialWellnessDbContext db,
        IMessageBus bus,
        CancellationToken ct
    )
    {
        UserSettingsDto? userSettings = await bus.InvokeAsync<UserSettingsDto?>(
            new GetUserSettingsRequest(principal.GetSubject()),
            ct
        );

        if (userSettings is null)
        {
            return Response<GetSnapshotMonthResponse>.Fail(
                HttpStatusCode.NotFound,
                "The user settings do not exist."
            );
        }

        Guid userId = userSettings.UserId;
        List<Snapshot> snapshots = await db
            .Snapshots.Where(snapshot => snapshot.UserId == userId)
            .Select(snapshot => new Snapshot
            {
                Id = snapshot.Id,
                Year = snapshot.Year,
                Month = snapshot.Month,
                SelfRating = snapshot.SelfRating,
                Amounts = snapshot.Amounts,
            })
            .ToListAsync(ct);
        SnapshotMonth wanted = new(year, month);
        int later = GetSnapshotMonthRules.CountLater(
            wanted,
            snapshots.Select(SnapshotHistoryMapping.MonthOf)
        );
        Snapshot? snapshot = snapshots.SingleOrDefault(stored =>
            SnapshotHistoryMapping.MonthOf(stored) == wanted
        );

        if (snapshot is not null)
        {
            SnapshotAmounts amounts = snapshot.Amounts;
            ExpenseAmounts expenses = amounts.Expenses;

            return Response<GetSnapshotMonthResponse>.Ok(
                new ActualMonthResponse(
                    snapshot.Id,
                    later,
                    amounts.Income,
                    new ExpensesResponse(
                        expenses.Food,
                        expenses.Housing,
                        expenses.Health,
                        expenses.Clothing,
                        expenses.Restaurants,
                        expenses.Transport,
                        expenses.Other
                    ),
                    amounts.Saved,
                    amounts.Reserves,
                    amounts.DebtPayments,
                    snapshot.SelfRating
                )
            );
        }

        EstimatedSnapshot? estimate = await db
            .EstimatedSnapshots.Where(stored =>
                stored.UserId == userId && stored.Year == year && stored.Month == month
            )
            .SingleOrDefaultAsync(ct);

        return estimate is null
            ? Response<GetSnapshotMonthResponse>.Fail(
                HttpStatusCode.NotFound,
                "The month has no snapshot."
            )
            : Response<GetSnapshotMonthResponse>.Ok(
                new EstimatedMonthResponse(
                    estimate.Id,
                    later,
                    new Money(estimate.ExpensesAmount, estimate.ExpensesCurrency),
                    new Money(estimate.SavedAmount, estimate.SavedCurrency)
                )
            );
    }
}
