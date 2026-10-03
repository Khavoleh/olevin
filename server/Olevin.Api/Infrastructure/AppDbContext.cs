using Microsoft.EntityFrameworkCore;

namespace Olevin.Api.Infrastructure;

/// <summary>
/// The application database. Entity configurations live next to their features and are picked up automatically.
/// </summary>
/// <param name="options">The options for this context.</param>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options);
