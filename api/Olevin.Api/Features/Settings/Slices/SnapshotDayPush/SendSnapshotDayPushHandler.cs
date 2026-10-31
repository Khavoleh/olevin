using System.Net;
using System.Text.Json;
using Lib.Net.Http.WebPush;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Features.Settings.Data;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;
using StoredSubscription = Olevin.Api.Features.Settings.Data.PushSubscriptions.PushSubscription;

namespace Olevin.Api.Features.Settings.Slices.SnapshotDayPush;

/// <summary>
/// Sends the push of a snapshot day to one browser.
/// </summary>
public static class SendSnapshotDayPushHandler
{
    private const string Kind = "SnapshotDay";

    /// <summary>
    /// The topic lets the push service replace a push that is not delivered yet instead of keeping both.
    /// </summary>
    private const string Topic = "snapshot-day";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Retries a push the push service could not take now, after a growing pause.
    /// </summary>
    /// <param name="chain">The handler chain of <see cref="SendSnapshotDayPush"/>.</param>
    public static void Configure(HandlerChain chain) =>
        chain
            .OnException<HttpRequestException>()
            .Or<PushServiceClientException>()
            .ScheduleRetry(
                TimeSpan.FromSeconds(30),
                TimeSpan.FromMinutes(2),
                TimeSpan.FromMinutes(10)
            );

    /// <summary>
    /// Handles <see cref="SendSnapshotDayPush"/> from the outbox. A subscription the push service no longer knows is
    /// deleted; a push whose snapshot day is over is dropped.
    /// </summary>
    /// <param name="message">The push to send.</param>
    /// <param name="db">The settings database.</param>
    /// <param name="client">The Web Push client.</param>
    /// <param name="time">The clock.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that completes when the push is sent or dropped.</returns>
    public static async Task Handle(
        SendSnapshotDayPush message,
        SettingsDbContext db,
        PushServiceClient client,
        TimeProvider time,
        ILogger logger,
        CancellationToken ct
    )
    {
        Target? target = await db
            .PushSubscriptions.Where(subscription =>
                subscription.Id == message.SubscriptionId && subscription.User.Setting != null
            )
            .Select(subscription => new Target(
                subscription,
                subscription.User.Setting!.Timezone,
                subscription.User.Setting.Locale
            ))
            .FirstOrDefaultAsync(ct);

        if (target is null)
            return;

        TimeSpan left = SnapshotDayPushSchedule.TimeLeft(
            message.SnapshotDay,
            time.GetUtcNow(),
            target.Timezone
        );

        if (left <= TimeSpan.Zero)
        {
            logger.LogInformation(
                "Dropped the push of the subscription {SubscriptionId}, its snapshot day is over.",
                message.SubscriptionId
            );
            return;
        }

        SnapshotDayPushPayload payload = new(
            Kind,
            message.SnapshotDay.Year,
            message.SnapshotDay.Month,
            target.Locale.ToString()
        );
        PushMessage push = new(JsonSerializer.Serialize(payload, Json))
        {
            Topic = Topic,
            TimeToLive = (int)left.TotalSeconds,
            Urgency = PushMessageUrgency.Normal,
        };

        try
        {
            await client.RequestPushMessageDeliveryAsync(ToWebPush(target.Subscription), push, ct);
        }
        catch (PushServiceClientException exception)
            when (exception.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone)
        {
            db.PushSubscriptions.Remove(target.Subscription);

            logger.LogInformation(
                exception,
                "Deleted the push subscription {SubscriptionId}, the push service no longer knows it.",
                message.SubscriptionId
            );
            return;
        }
        catch (PushServiceClientException exception) when (!IsTransient(exception.StatusCode))
        {
            logger.LogError(
                exception,
                "The push service refused the push of the subscription {SubscriptionId} with {StatusCode}.",
                message.SubscriptionId,
                (int)exception.StatusCode
            );
            return;
        }

        logger.LogInformation(
            "Sent the snapshot day push to the subscription {SubscriptionId}.",
            message.SubscriptionId
        );
    }

    private static bool IsTransient(HttpStatusCode status) =>
        status == HttpStatusCode.TooManyRequests || (int)status >= 500;

    private static PushSubscription ToWebPush(StoredSubscription stored)
    {
        PushSubscription subscription = new() { Endpoint = stored.Endpoint };
        subscription.SetKey(PushEncryptionKeyName.P256DH, stored.P256dh);
        subscription.SetKey(PushEncryptionKeyName.Auth, stored.Auth);

        return subscription;
    }

    private sealed record Target(
        StoredSubscription Subscription,
        string Timezone,
        LocaleCode Locale
    );
}
