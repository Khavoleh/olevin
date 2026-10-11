using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data.FxRates;

namespace Olevin.Api.Features.FinancialWellness.Data;

/// <summary>
/// The database context of the Financial Wellness feature, bound to the <c>financial_wellness</c> schema.
/// </summary>
/// <param name="options">The options for this context.</param>
public sealed class FinancialWellnessDbContext(DbContextOptions<FinancialWellnessDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// Gets the daily exchange rates.
    /// </summary>
    public DbSet<FxRate> FxRates => Set<FxRate>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new FxRateConfiguration());
}
