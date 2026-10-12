using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Olevin.Api.Features.FinancialWellness.Data.FxRates;

/// <summary>
/// Maps <see cref="FxRate"/> to <c>financial_wellness.fx_rates</c>.
/// </summary>
public sealed class FxRateConfiguration : IEntityTypeConfiguration<FxRate>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<FxRate> builder)
    {
        builder.HasKey(rate => new { rate.Currency, rate.RateDate }).HasName("fx_rates_pkey");

        builder.ToTable(
            "fx_rates",
            "financial_wellness",
            table =>
                table.HasComment(
                    "Daily exchange rates of currencies against the base currency (USD)"
                )
        );

        builder
            .Property(rate => rate.Currency)
            .HasComment("Currency the rate is quoted for")
            .HasColumnName("currency");
        builder
            .Property(rate => rate.UnitsPerBase)
            .HasPrecision(18, 8)
            .HasComment("Amount of the currency for one unit of the base currency (USD)")
            .HasColumnName("units_per_base");
        builder
            .Property(rate => rate.RateDate)
            .HasDefaultValueSql("current_date")
            .HasComment("Date the rate is valid for")
            .HasColumnName("rate_date");
    }
}
