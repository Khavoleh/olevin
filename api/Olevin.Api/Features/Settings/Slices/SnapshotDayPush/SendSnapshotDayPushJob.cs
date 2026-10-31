using System.Diagnostics;
using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.PushSubscriptions;
using Olevin.Api.Infrastructure.Telemetry;
using Wolverine.EntityFrameworkCore;

namespace Olevin.Api.Features.Settings.Slices.SnapshotDayPush;

/// <summary>
/// Queues the push of the snapshot day for every browser whose user has reached the noon of it. The job runs every few
/// minutes; each subscription is marked with the month in the same transaction as its message in the outbox, so a
/// browser gets one push a month.
/// </summary>
/// <param name="outbox">The settings database with the Wolverine outbox.</param>
/// <param name="time">The clock.</param>
/// <param name="logger">The logger.</param>
public sealed class SendSnapshotDayPushJob(
    IDbContextOutbox<SettingsDbContext> outbox,
    TimeProvider time,
    ILogger<SendSnapshotDayPushJob> logger
) : IInvocable, ICancellableInvocable
{
    private static readonly ActivitySource Source = new(Telemetry.ActivitySourceName);

    /// <summary>
    /// Gets or sets the token that is cancelled when the application shuts down. Set by Coravel.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Queues the pushes that are due now.
    /// </summary>
    /// <returns>A task that completes when the run ends.</returns>
    public async Task Invoke()
    {
        using Activity? activity = Source.StartActivity("Queue snapshot day pushes");

        try
        {
            await QueueAsync();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            logger.LogError(exception, "Queueing the snapshot day pushes failed.");
        }
    }

    private async Task QueueAsync()
    {
        DateTimeOffset now = time.GetUtcNow();

        List<Recipient> recipients = await outbox
            .DbContext.PushSubscriptions.Where(subscription => subscription.User.Setting != null)
            .Select(subscription => new Recipient(
                subscription,
                subscription.User.Setting!.Timezone,
                subscription.User.Setting.SnapshotDay
            ))
            .ToListAsync(CancellationToken);

        int queued = 0;

        foreach (Recipient recipient in recipients)
        {
            PushSubscription subscription = recipient.Subscription;
            DateOnly? due = SnapshotDayPushSchedule.DueDate(
                now,
                recipient.Timezone,
                recipient.SnapshotDay
            );

            if (
                due is not { } snapshotDay
                || SnapshotDayPushSchedule.IsSent(
                    snapshotDay,
                    subscription.SnapshotPushYear,
                    subscription.SnapshotPushMonth
                )
            )
            {
                continue;
            }

            subscription.SnapshotPushYear = (short)snapshotDay.Year;
            subscription.SnapshotPushMonth = (CalendarMonth)snapshotDay.Month;

            await outbox.PublishAsync(new SendSnapshotDayPush(subscription.Id, snapshotDay));
            queued++;
        }

        if (queued == 0)
            return;

        await outbox.SaveChangesAndFlushMessagesAsync(CancellationToken);

        logger.LogInformation("Queued {Count} snapshot day pushes.", queued);
    }

    private sealed record Recipient(
        PushSubscription Subscription,
        string Timezone,
        short SnapshotDay
    );
}
