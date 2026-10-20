using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Olevin.Api.Features.Settings.Data.CfpbResponses;

/// <summary>
/// Maps <see cref="CfpbResponse"/> to <c>settings.cfpb_responses</c>; mirrors <c>infrastructure/db/schemas/settings.hcl</c>.
/// </summary>
public sealed class CfpbResponseConfiguration : IEntityTypeConfiguration<CfpbResponse>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<CfpbResponse> builder)
    {
        builder.HasKey(response => response.UserId).HasName("cfpb_responses_pkey");

        builder.ToTable(
            "cfpb_responses",
            "settings",
            table =>
                table.HasComment("CFPB Financial Well-Being Scale answers given once at onboarding")
        );

        builder
            .Property(response => response.UserId)
            .ValueGeneratedNever()
            .HasComment("Owning user identifier")
            .HasColumnName("user_id");
        builder
            .Property(response => response.Answers)
            .HasComment("Response values of the 10 statements in order")
            .HasColumnName("answers");
        builder
            .Property(response => response.Is62Plus)
            .HasComment("Whether the respondent is 62 or older")
            .HasColumnName("is_62_plus");
        builder
            .Property(response => response.Score)
            .HasComment("Financial well-being score from the self-administered conversion table")
            .HasColumnName("score");
        builder
            .Property(response => response.SubmittedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the response was submitted")
            .HasColumnName("submitted_at");

        builder
            .HasOne(response => response.User)
            .WithOne(user => user.CfpbResponse)
            .HasForeignKey<CfpbResponse>(response => response.UserId)
            .HasConstraintName("cfpb_responses_user_id_fkey");
    }
}
