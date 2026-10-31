using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Wolverine.Attributes;

namespace Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;

/// <summary>
/// Gives other features the user with their settings.
/// </summary>
public static class GetUserSettingsHandler
{
    /// <summary>
    /// Handles <see cref="GetUserSettingsRequest"/>.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The user with their settings, or <see langword="null"/> when the user or their settings don't exist.</returns>
    /// <remarks>It only reads, so it opts out of the automatic transactions.</remarks>
    [NonTransactional]
    public static async Task<UserSettingsDto?> Handle(
        GetUserSettingsRequest request,
        SettingsDbContext db,
        CancellationToken ct
    ) =>
        await db
            .UserSettings.Where(setting => setting.User.AuthSub == request.AuthSub)
            .Select(setting => new UserSettingsDto(
                setting.UserId,
                setting.BaseCurrency,
                setting.SnapshotDay,
                setting.Timezone
            ))
            .FirstOrDefaultAsync(ct);
}
