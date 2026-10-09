using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Olevin.Api.Features.Settings.Data.Users;

/// <summary>
/// Maps <see cref="User"/> to <c>settings.users</c>; mirrors <c>infrastructure/db/schemas/settings.hcl</c>.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id).HasName("users_pkey");

        builder.ToTable("users", "settings", table => table.HasComment("Application user profile"));

        builder.HasIndex(user => user.AuthSub, "users_auth_sub_key").IsUnique();

        builder
            .Property(user => user.Id)
            .HasDefaultValueSql("uuidv7()")
            .HasComment("User identifier")
            .HasColumnName("id");
        builder
            .Property(user => user.AuthSub)
            .HasMaxLength(128)
            .HasComment("Logto sub claim")
            .HasColumnName("auth_sub");
        builder
            .Property(user => user.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the profile was created")
            .HasColumnName("created_at");
        builder
            .Property(user => user.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the profile was last changed")
            .HasColumnName("updated_at");
    }
}
