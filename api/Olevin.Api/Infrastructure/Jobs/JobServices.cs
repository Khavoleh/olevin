using Coravel;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Olevin.Api.Infrastructure.Jobs;

/// <summary>
/// Registers the background job scheduler (Coravel) shared by the features. Each feature adds its own jobs and
/// schedules them with <c>UseScheduler</c> once the application is built.
/// </summary>
public static class JobServices
{
    /// <summary>
    /// Adds the Coravel scheduler, which runs the scheduled jobs in the background, and the system clock the jobs use to
    /// tell the time. The scheduler starts ticking only after the application has fully started, so the jobs never run
    /// while the startup work (such as seeding) is still in progress.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddJobs(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddScheduler();

        return services;
    }
}
