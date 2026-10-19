using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Attributes;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.GetBaseCurrency;

/// <summary>
/// Gives the base currency the signed-in user saved during onboarding.
/// </summary>
[Tags("Onboarding")]
public static class GetBaseCurrencyEndpoint
{
    /// <summary>
    /// Handles <c>GET /settings/v1/user-settings/base-currency</c>.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The base currency of the user (200), or a not found (404) when the user or their settings don't exist.
    /// </returns>
    [WolverineGet("/settings/v1/user-settings/base-currency", OperationId = "GetBaseCurrency")]
    [ProducesResponseType<Response<CurrencyCode>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<CurrencyCode>>(StatusCodes.Status404NotFound)]
    [NonTransactional]
    public static async Task<Response<CurrencyCode>> Get(
        ClaimsPrincipal principal,
        SettingsDbContext db,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        CurrencyCode? baseCurrency = await db
            .UserSettings.Where(setting => setting.User.AuthSub == authSub)
            .Select(setting => (CurrencyCode?)setting.BaseCurrency)
            .FirstOrDefaultAsync(ct);

        return baseCurrency is { } currency
            ? Response<CurrencyCode>.Ok(currency)
            : Response<CurrencyCode>.Fail(HttpStatusCode.NotFound, "The settings do not exist.");
    }
}
