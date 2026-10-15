using CivicConnect.AuditLog.Events;
using CivicConnect.Common;
using CivicConnect.Notifications;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CivicConnect.Tests.Notifications;

public class NotificationsModuleTests
{
    private static RequestStatusChangedEvent Event() => new(12, "Submitted", "Assigned", 10, DateTimeOffset.UtcNow);

    [Fact]
    public void The_module_alone_registers_the_silent_service()
    {
        var provider = new ServiceCollection().AddNotificationsModule().BuildServiceProvider();

        Assert.IsType<NullNotificationService>(provider.GetRequiredService<INotificationService>());
    }

    [Fact]
    public void Adding_the_message_box_after_the_module_replaces_the_silent_service()
    {
        // Same order as Program.cs: the last registration wins.
        var provider = new ServiceCollection()
            .AddNotificationsModule()
            .AddMessageBoxNotification()
            .BuildServiceProvider();

        Assert.IsType<MessageBoxNotificationService>(provider.GetRequiredService<INotificationService>());
    }

    [Fact]
    public void The_module_registers_the_status_change_handler()
    {
        var provider = new ServiceCollection().AddNotificationsModule().BuildServiceProvider();

        var handler = Assert.Single(provider.GetServices<IDomainEventHandler<RequestStatusChangedEvent>>());
        Assert.IsType<NotificationEventHandler>(handler);
    }

    [Fact]
    public async Task A_dispatched_status_change_reaches_the_notification_channel()
    {
        var notifications = new RecordingNotificationService();
        var services = new ServiceCollection().AddNotificationsModule();
        services.AddSingleton<INotificationService>(notifications);   // replaces the silent default
        var provider = services.BuildServiceProvider();

        await new DomainEventDispatcher(provider).DispatchAsync(Event());

        var message = Assert.Single(notifications.Messages);
        Assert.Contains("#12", message.Body);
        Assert.Contains("Assigned", message.Body);
    }
}
