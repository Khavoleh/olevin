using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetBaseCurrency;

/// <summary>
/// Gives the base currency the amounts of a new snapshot default to.
/// </summary>
[Tags("Snapshots")]
public static class GetBaseCurrencyEndpoint
{
    /// <summary>
    /// Handles <c>GET /financial-wellness/v1/snapshots/base-currency</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="bus">The message bus.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The base currency of the user (200), or a not found (404) without user settings.</returns>
    [WolverineGet(
        "/financial-wellness/v1/snapshots/base-currency",
        OperationId = "GetSnapshotBaseCurrency"
    )]
    [ProducesResponseType<Response<CurrencyCode>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<CurrencyCode>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<CurrencyCode>> Get(
        ClaimsPrincipal principal,
        IMessageBus bus,
        CancellationToken ct
    )
    {
        UserSettingsDto? userSettings = await bus.InvokeAsync<UserSettingsDto?>(
            new GetUserSettingsRequest(principal.GetSubject()),
            ct
        );

        return userSettings is null
            ? Response<CurrencyCode>.Fail(
                HttpStatusCode.NotFound,
                "The user settings do not exist."
            )
            : Response<CurrencyCode>.Ok(userSettings.BaseCurrency);
    }
}
