using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.FxRates;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;

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

    /// <summary>
    /// Gets the monthly snapshots.
    /// </summary>
    public DbSet<Snapshot> Snapshots => Set<Snapshot>();

    /// <summary>
    /// Gets the rough onboarding months.
    /// </summary>
    public DbSet<EstimatedSnapshot> EstimatedSnapshots => Set<EstimatedSnapshot>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FxRateConfiguration());
        modelBuilder.ApplyConfiguration(new SnapshotConfiguration());
        modelBuilder.ApplyConfiguration(new EstimatedSnapshotConfiguration());
    }
}
