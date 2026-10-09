using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data.Users;
using Olevin.Api.Features.Settings.Data.UserSettings;

namespace Olevin.Api.Features.Settings.Data;

/// <summary>
/// The database context of the Settings feature, bound to the <c>settings</c> schema.
/// </summary>
/// <param name="options">The options for this context.</param>
public sealed class SettingsDbContext(DbContextOptions<SettingsDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// Gets the user profiles.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets the user settings.
    /// </summary>
    public DbSet<UserSetting> UserSettings => Set<UserSetting>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserSettingConfiguration());
    }
}
