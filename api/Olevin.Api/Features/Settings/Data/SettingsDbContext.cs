using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data.CfpbResponses;
using Olevin.Api.Features.Settings.Data.PushSubscriptions;
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

    /// <summary>
    /// Gets the CFPB Financial Well-Being Scale responses.
    /// </summary>
    public DbSet<CfpbResponse> CfpbResponses => Set<CfpbResponse>();

    /// <summary>
    /// Gets the Web Push subscriptions.
    /// </summary>
    public DbSet<PushSubscription> PushSubscriptions => Set<PushSubscription>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserSettingConfiguration());
        modelBuilder.ApplyConfiguration(new CfpbResponseConfiguration());
        modelBuilder.ApplyConfiguration(new PushSubscriptionConfiguration());
    }
}
