using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;

/// <summary>
/// Maps <see cref="EstimatedSnapshot"/> to <c>financial_wellness.estimated_snapshots</c>; mirrors
/// <c>infrastructure/db/schemas/financial_wellness.hcl</c>.
/// </summary>
public sealed class EstimatedSnapshotConfiguration : IEntityTypeConfiguration<EstimatedSnapshot>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<EstimatedSnapshot> builder)
    {
        builder.HasKey(snapshot => snapshot.Id).HasName("estimated_snapshots_pkey");

        builder.ToTable(
            "estimated_snapshots",
            "financial_wellness",
            table =>
                table.HasComment(
                    "Rough months entered at onboarding before the first snapshot; never removed"
                )
        );

        builder
            .HasIndex(
                snapshot => new
                {
                    snapshot.UserId,
                    snapshot.Year,
                    snapshot.Month,
                },
                "estimated_snapshots_user_id_year_month_key"
            )
            .IsUnique();

        builder
            .Property(snapshot => snapshot.Id)
            .HasDefaultValueSql("uuidv7()")
            .HasComment("Estimated snapshot identifier")
            .HasColumnName("id");
        builder
            .Property(snapshot => snapshot.UserId)
            .HasComment("Owning user identifier")
            .HasColumnName("user_id");
        builder
            .Property(snapshot => snapshot.Month)
            .HasComment("Month of the estimated snapshot")
            .HasColumnName("month");
        builder
            .Property(snapshot => snapshot.Year)
            .HasComment("Year of the estimated snapshot")
            .HasColumnName("year");
        builder
            .Property(snapshot => snapshot.FxDate)
            .HasComment("Date of the exchange rates the amounts are converted at")
            .HasColumnName("fx_date");
        builder
            .Property(snapshot => snapshot.ExpensesCurrency)
            .HasComment("Currency the total expenses were entered in")
            .HasColumnName("expenses_currency");
        builder
            .Property(snapshot => snapshot.ExpensesAmount)
            .HasPrecision(15, 2)
            .HasComment("Total expenses of the month")
            .HasColumnName("expenses_amount");
        builder
            .Property(snapshot => snapshot.SavedCurrency)
            .HasComment("Currency the saved amount was entered in")
            .HasColumnName("saved_currency");
        builder
            .Property(snapshot => snapshot.SavedAmount)
            .HasPrecision(15, 2)
            .HasComment("Amount saved in the month")
            .HasColumnName("saved_amount");
        builder
            .Property(snapshot => snapshot.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the estimated snapshot was created")
            .HasColumnName("created_at");
        builder
            .Property(snapshot => snapshot.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the estimated snapshot was last changed")
            .HasColumnName("updated_at");
        builder.Property(snapshot => snapshot.Version).IsRowVersion();
    }
}
