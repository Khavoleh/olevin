using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Olevin.Api.Features.Settings.Data.PushSubscriptions;

/// <summary>
/// Maps <see cref="PushSubscription"/> to <c>settings.push_subscriptions</c>; mirrors
/// <c>infrastructure/db/schemas/settings.hcl</c>.
/// </summary>
public sealed class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.HasKey(subscription => subscription.Id).HasName("push_subscriptions_pkey");

        builder.ToTable(
            "push_subscriptions",
            "settings",
            table => table.HasComment("Web Push subscriptions of the user's browsers and devices")
        );

        builder
            .HasIndex(subscription => subscription.Endpoint, "push_subscriptions_endpoint_key")
            .IsUnique();
        builder.HasIndex(subscription => subscription.UserId, "push_subscriptions_user_id_idx");

        builder
            .Property(subscription => subscription.Id)
            .HasDefaultValueSql("uuidv7()")
            .HasComment("Subscription identifier")
            .HasColumnName("id");
        builder
            .Property(subscription => subscription.UserId)
            .HasComment("Owning user identifier")
            .HasColumnName("user_id");
        builder
            .Property(subscription => subscription.Endpoint)
            .HasMaxLength(2048)
            .HasComment("URL of the push service the messages are sent to")
            .HasColumnName("endpoint");
        builder
            .Property(subscription => subscription.P256dh)
            .HasMaxLength(128)
            .HasComment("Public key of the browser the payload is encrypted for")
            .HasColumnName("p256dh");
        builder
            .Property(subscription => subscription.Auth)
            .HasMaxLength(64)
            .HasComment("Authentication secret of the browser")
            .HasColumnName("auth");
        builder
            .Property(subscription => subscription.DeviceName)
            .HasMaxLength(128)
            .HasComment("Name of the browser and device shown to the user")
            .HasColumnName("device_name");
        builder
            .Property(subscription => subscription.SnapshotPushYear)
            .HasComment("Year of the snapshot day push last sent to the subscription")
            .HasColumnName("snapshot_push_year");
        builder
            .Property(subscription => subscription.SnapshotPushMonth)
            .HasComment("Month of the snapshot day push last sent to the subscription")
            .HasColumnName("snapshot_push_month");
        builder
            .Property(subscription => subscription.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the subscription was created")
            .HasColumnName("created_at");
        builder
            .Property(subscription => subscription.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the browser last confirmed the subscription")
            .HasColumnName("updated_at");

        builder
            .HasOne(subscription => subscription.User)
            .WithMany(user => user.PushSubscriptions)
            .HasForeignKey(subscription => subscription.UserId)
            .HasConstraintName("push_subscriptions_user_id_fkey");
    }
}
