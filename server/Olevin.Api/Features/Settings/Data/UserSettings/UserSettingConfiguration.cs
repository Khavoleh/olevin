using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Olevin.Api.Features.Settings.Data.UserSettings;

/// <summary>
/// Maps <see cref="UserSetting"/> to <c>settings.user_settings</c>; mirrors <c>infrastructure/db/schemas/settings.hcl</c>.
/// </summary>
public sealed class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        builder.HasKey(setting => setting.UserId).HasName("user_settings_pkey");

        builder.ToTable(
            "user_settings",
            "settings",
            table => table.HasComment("Per-user application settings (one row per user)")
        );

        builder
            .Property(setting => setting.UserId)
            .ValueGeneratedNever()
            .HasComment("Owning user identifier")
            .HasColumnName("user_id");
        builder
            .Property(setting => setting.BaseCurrency)
            .HasComment("Default user's currency")
            .HasColumnName("base_currency");
        builder
            .Property(setting => setting.Locale)
            .HasComment("Interface language")
            .HasColumnName("locale");
        builder
            .Property(setting => setting.SnapshotDay)
            .HasComment("Day of the month the monthly snapshot is taken on (1-31)")
            .HasColumnName("snapshot_day");
        builder
            .Property(setting => setting.Timezone)
            .HasMaxLength(64)
            .HasComment("IANA time zone name")
            .HasColumnName("timezone");
        builder
            .Property(setting => setting.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the settings were created")
            .HasColumnName("created_at");
        builder
            .Property(setting => setting.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the settings were last changed")
            .HasColumnName("updated_at");

        builder
            .HasOne(setting => setting.User)
            .WithOne(user => user.Setting)
            .HasForeignKey<UserSetting>(setting => setting.UserId)
            .HasConstraintName("user_settings_user_id_fkey");
    }
}
