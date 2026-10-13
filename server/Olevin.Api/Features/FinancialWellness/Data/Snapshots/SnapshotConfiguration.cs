using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olevin.Api.Shared.Data;

namespace Olevin.Api.Features.FinancialWellness.Data.Snapshots;

/// <summary>
/// Maps <see cref="Snapshot"/> to <c>financial_wellness.snapshots</c>; mirrors
/// <c>infrastructure/db/schemas/financial_wellness.hcl</c>.
/// </summary>
public sealed class SnapshotConfiguration : IEntityTypeConfiguration<Snapshot>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Snapshot> builder)
    {
        builder.HasKey(snapshot => snapshot.Id).HasName("snapshots_pkey");

        builder.ToTable(
            "snapshots",
            "financial_wellness",
            table =>
                table.HasComment(
                    "Monthly snapshots of a user's finances: the entered amounts and the index calculated from them"
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
                "snapshots_user_id_year_month_key"
            )
            .IsUnique();

        builder
            .Property(snapshot => snapshot.Id)
            .HasDefaultValueSql("uuidv7()")
            .HasComment("Snapshot identifier")
            .HasColumnName("id");
        builder
            .Property(snapshot => snapshot.UserId)
            .HasComment("Owning user identifier")
            .HasColumnName("user_id");
        builder
            .Property(snapshot => snapshot.Month)
            .HasComment("Month of the snapshot")
            .HasColumnName("month");
        builder
            .Property(snapshot => snapshot.Year)
            .HasComment("Year of the snapshot")
            .HasColumnName("year");
        builder
            .Property(snapshot => snapshot.FxDate)
            .HasComment("Date of the exchange rates the amounts are converted at")
            .HasColumnName("fx_date");
        builder
            .Property(snapshot => snapshot.BaseCurrency)
            .HasComment("Currency the results were calculated in")
            .HasColumnName("base_currency");
        builder
            .Property(snapshot => snapshot.SelfRating)
            .HasComment("Self-rated financial calm (1-5)")
            .HasColumnName("self_rating");
        builder
            .Property(snapshot => snapshot.SavingsRate)
            .HasComment("Savings rate (SR), clamped to its range")
            .HasColumnName("savings_rate");
        builder
            .Property(snapshot => snapshot.ReserveMonths)
            .HasComment("Liquid reserves in months of spending (R), clamped to its range")
            .HasColumnName("reserve_months");
        builder
            .Property(snapshot => snapshot.DebtToIncome)
            .HasComment("Debt payments to income (DTI), clamped to its range")
            .HasColumnName("debt_to_income");
        builder
            .Property(snapshot => snapshot.ExpenseVariation)
            .HasComment("Coefficient of variation of expenses (CV), clamped to its range")
            .HasColumnName("expense_variation");
        builder
            .Property(snapshot => snapshot.SavingsRegularity)
            .HasComment("Share of recent months with savings (RG)")
            .HasColumnName("savings_regularity");
        builder
            .Property(snapshot => snapshot.CashFlow)
            .HasComment("Output of the cash flow subsystem (0-100)")
            .HasColumnName("cash_flow");
        builder
            .Property(snapshot => snapshot.Buffer)
            .HasComment("Output of the financial buffer subsystem (0-100)")
            .HasColumnName("buffer");
        builder
            .Property(snapshot => snapshot.FuzzyIndex)
            .HasComment("Fuzzy index (0-100)")
            .HasColumnName("i_fuzzy");
        builder
            .Property(snapshot => snapshot.LinearIndex)
            .HasComment("Linear index for comparison, not shown to the user (0-100)")
            .HasColumnName("i_lin");
        builder
            .Property(snapshot => snapshot.FinalIndex)
            .HasComment("Final index corrected by the self-rating (0-100)")
            .HasColumnName("i_final");
        builder
            .Property(snapshot => snapshot.Amounts)
            .HasJsonbConversion()
            .HasComment("Entered amounts, each in the currency it was entered in")
            .HasColumnName("amounts");
        builder
            .Property(snapshot => snapshot.ModelVersion)
            .HasMaxLength(16)
            .HasComment("Version of the index model the results were calculated with")
            .HasColumnName("model_version");
        builder
            .Property(snapshot => snapshot.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the snapshot was created")
            .HasColumnName("created_at");
        builder
            .Property(snapshot => snapshot.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasComment("When the snapshot was last changed")
            .HasColumnName("updated_at");
        builder.Property(snapshot => snapshot.Version).IsRowVersion();
    }
}
