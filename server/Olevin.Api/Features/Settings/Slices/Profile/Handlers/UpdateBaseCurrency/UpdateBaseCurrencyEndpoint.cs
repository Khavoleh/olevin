using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.RescoreSnapshots;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateBaseCurrency;

/// <summary>
/// Changes the base currency of the signed-in user.
/// </summary>
[Tags("Profile")]
public static class UpdateBaseCurrencyEndpoint
{
    /// <summary>
    /// Handles <c>PUT /settings/v1/user-settings/base-currency</c>. The new currency is saved together with a message
    /// in the outbox, which scores every snapshot again after the commit.
    /// </summary>
    /// <param name="request">The new base currency.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="bus">The message bus, to score the snapshots again in the financial wellness feature.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The saved base currency (200), or a not found (404) when the user or their settings don't exist.
    /// </returns>
    [WolverinePut("/settings/v1/user-settings/base-currency", OperationId = "UpdateBaseCurrency")]
    [ProducesResponseType<Response<CurrencyCode>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<CurrencyCode>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<CurrencyCode>> Put(
        UpdateBaseCurrencyRequest request,
        ClaimsPrincipal principal,
        SettingsDbContext db,
        IMessageBus bus,
        TimeProvider time,
        ILogger logger,
        CancellationToken ct
    )
    {
        string authSub = principal.GetSubject();

        UserSetting? setting = await db.UserSettings.FirstOrDefaultAsync(
            setting => setting.User.AuthSub == authSub,
            ct
        );

        if (setting is null)
        {
            return Response<CurrencyCode>.Fail(
                HttpStatusCode.NotFound,
                "The settings do not exist."
            );
        }

        if (setting.BaseCurrency == request.BaseCurrency)
            return Response<CurrencyCode>.Ok(setting.BaseCurrency);

        setting.BaseCurrency = request.BaseCurrency;
        setting.UpdatedAt = time.GetUtcNow().UtcDateTime;

        await bus.PublishAsync(new RescoreSnapshotsRequest(setting.UserId, setting.BaseCurrency));

        logger.LogInformation("Updated the base currency of the user {UserId}.", setting.UserId);

        return Response<CurrencyCode>.Ok(setting.BaseCurrency);
    }
}
