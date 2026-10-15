using CivicConnect.Common;
using CivicConnect.Notifications;
using System;
using System.Threading.Tasks;

namespace CivicConnect.Tests.Notifications;

public class NotificationEventHandlerTests
{
    private static RequestStatusChangedEvent Event(string? oldStatus = "Submitted", string newStatus = "Assigned") =>
        new(12, oldStatus, newStatus, 10, DateTimeOffset.UtcNow);

    [Fact]
    public async Task A_status_change_shows_exactly_one_notification()
    {
        var notifications = new RecordingNotificationService();

        await new NotificationEventHandler(notifications).HandleAsync(Event());

        Assert.Single(notifications.Messages);
    }

    [Fact]
    public async Task The_notification_names_the_request_and_both_statuses()
    {
        var notifications = new RecordingNotificationService();

        await new NotificationEventHandler(notifications).HandleAsync(Event("Assigned", "InProgress"));

        var body = Assert.Single(notifications.Messages).Body;
        Assert.Contains("#12", body);
        Assert.Contains("Assigned", body);
        Assert.Contains("InProgress", body);
    }

    [Fact]
    public async Task A_missing_old_status_reads_as_New()
    {
        var notifications = new RecordingNotificationService();

        await new NotificationEventHandler(notifications).HandleAsync(Event(oldStatus: null, newStatus: "Submitted"));

        Assert.Contains("from New to Submitted", Assert.Single(notifications.Messages).Body);
    }

    [Fact]
    public async Task The_notification_has_a_title_and_is_informational()
    {
        var notifications = new RecordingNotificationService();

        await new NotificationEventHandler(notifications).HandleAsync(Event());

        var message = Assert.Single(notifications.Messages);
        Assert.False(string.IsNullOrWhiteSpace(message.Title));
        Assert.Equal(NotificationSeverity.Info, message.Severity);
    }

    [Fact]
    public async Task The_notification_never_claims_an_email_was_sent()
    {
        var notifications = new RecordingNotificationService();

        await new NotificationEventHandler(notifications).HandleAsync(Event());

        var message = Assert.Single(notifications.Messages);
        Assert.DoesNotContain("email", message.Title, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("email", message.Body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void A_null_event_is_rejected()
    {
        var handler = new NotificationEventHandler(new RecordingNotificationService());

        Assert.Throws<ArgumentNullException>(() => { _ = handler.HandleAsync(null!); });
    }

    [Fact]
    public void A_failing_notification_channel_is_not_hidden_by_the_handler()
    {
        var handler = new NotificationEventHandler(new ThrowingNotificationService());

        Assert.Throws<InvalidOperationException>(() => { _ = handler.HandleAsync(Event()); });
    }
}
