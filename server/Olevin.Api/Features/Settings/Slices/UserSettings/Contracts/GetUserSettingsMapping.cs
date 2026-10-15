using Olevin.Api.Features.Settings.Data.UserSettings;
using Riok.Mapperly.Abstractions;

namespace Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;

/// <summary>
/// Projects the stored settings into <see cref="UserSettingsDto"/>.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class GetUserSettingsMapping
{
    /// <summary>
    /// Projects the settings with their users in the query itself, so that only the columns of the DTO are read.
    /// </summary>
    /// <param name="query">The settings.</param>
    /// <returns>The users with their settings.</returns>
    public static partial IQueryable<UserSettingsDto> ProjectToDto(
        this IQueryable<UserSetting> query
    );
}
