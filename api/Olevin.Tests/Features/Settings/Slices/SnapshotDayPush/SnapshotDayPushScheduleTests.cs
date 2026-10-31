using Olevin.Api.Features.Settings.Data.PushSubscriptions;
using Olevin.Api.Features.Settings.Slices.SnapshotDayPush;

namespace Olevin.Tests.Features.Settings.Slices.SnapshotDayPush;

public sealed class SnapshotDayPushScheduleTests
{
    private const string Kyiv = "Europe/Kyiv";

    private static DateTimeOffset Utc(int year, int month, int day, int hour, int minute = 0) =>
        new(year, month, day, hour, minute, 0, TimeSpan.Zero);

    [Fact]
    public void DueDate_AtNoonOfTheSnapshotDay_IsToday() =>
        Assert.Equal(
            new DateOnly(2026, 6, 15),
            SnapshotDayPushSchedule.DueDate(Utc(2026, 6, 15, 9), Kyiv, 15)
        );

    [Fact]
    public void DueDate_BeforeNoon_IsNull() =>
        Assert.Null(SnapshotDayPushSchedule.DueDate(Utc(2026, 6, 15, 8, 59), Kyiv, 15));

    [Fact]
    public void DueDate_LateInTheSnapshotDay_IsToday() =>
        Assert.Equal(
            new DateOnly(2026, 6, 15),
            SnapshotDayPushSchedule.DueDate(Utc(2026, 6, 15, 20, 59), Kyiv, 15)
        );

    [Fact]
    public void DueDate_OnAnotherDay_IsNull() =>
        Assert.Null(SnapshotDayPushSchedule.DueDate(Utc(2026, 6, 16, 9), Kyiv, 15));

    [Fact]
    public void DueDate_InAShorterMonth_IsItsLastDay() =>
        Assert.Equal(
            new DateOnly(2026, 2, 28),
            SnapshotDayPushSchedule.DueDate(Utc(2026, 2, 28, 10), Kyiv, 31)
        );

    [Fact]
    public void DueDate_FollowsTheLocalDate_NotTheUtcOne() =>
        Assert.Equal(
            new DateOnly(2026, 6, 15),
            SnapshotDayPushSchedule.DueDate(Utc(2026, 6, 16, 2), "America/Los_Angeles", 15)
        );

    [Theory]
    [InlineData(29, false)]
    [InlineData(30, true)]
    public void DueDate_InAHalfHourZone_StartsAtItsNoon(int minute, bool isDue) =>
        Assert.Equal(
            isDue,
            SnapshotDayPushSchedule.DueDate(Utc(2026, 6, 15, 6, minute), "Asia/Kolkata", 15)
                is not null
        );

    [Fact]
    public void IsSent_InTheSameMonth_IsTrue() =>
        Assert.True(
            SnapshotDayPushSchedule.IsSent(new DateOnly(2026, 6, 15), 2026, CalendarMonth.June)
        );

    [Theory]
    [InlineData((short)2026, CalendarMonth.May)]
    [InlineData((short)2025, CalendarMonth.June)]
    [InlineData(null, null)]
    public void IsSent_InAnotherMonth_IsFalse(short? year, CalendarMonth? month) =>
        Assert.False(SnapshotDayPushSchedule.IsSent(new DateOnly(2026, 6, 15), year, month));

    [Fact]
    public void TimeLeft_IsTheRestOfTheLocalDay() =>
        Assert.Equal(
            TimeSpan.FromHours(12),
            SnapshotDayPushSchedule.TimeLeft(new DateOnly(2026, 6, 15), Utc(2026, 6, 15, 9), Kyiv)
        );

    [Fact]
    public void TimeLeft_AfterTheDay_IsNegative() =>
        Assert.Equal(
            TimeSpan.FromMinutes(-30),
            SnapshotDayPushSchedule.TimeLeft(
                new DateOnly(2026, 6, 15),
                Utc(2026, 6, 15, 21, 30),
                Kyiv
            )
        );
}
