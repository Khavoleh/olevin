using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateLocale;

/// <summary>
/// Changes the saved interface language of the signed-in user.
/// </summary>
[Tags("Profile")]
public static class UpdateLocaleEndpoint
{
    /// <summary>
    /// Handles <c>PUT /settings/v1/user-settings/locale</c>.
    /// </summary>
    /// <param name="request">The new interface language.</param>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The saved language (200), or a not found (404) when the user or their settings don't exist.
    /// </returns>
    [WolverinePut("/settings/v1/user-settings/locale", OperationId = "UpdateLocale")]
    [ProducesResponseType<Response<LocaleCode>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Response<LocaleCode>>(StatusCodes.Status404NotFound)]
    public static async Task<Response<LocaleCode>> Put(
        UpdateLocaleRequest request,
        ClaimsPrincipal principal,
        SettingsDbContext db,
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
            return Response<LocaleCode>.Fail(HttpStatusCode.NotFound, "The settings do not exist.");

        setting.Locale = request.Locale;
        setting.UpdatedAt = time.GetUtcNow().UtcDateTime;

        logger.LogInformation("Updated the locale of the user {UserId}.", setting.UserId);

        return Response<LocaleCode>.Ok(setting.Locale);
    }
}
