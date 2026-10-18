using CivicConnect.AuditLog;
using CivicConnect.AuditLog.Events;
using CivicConnect.Common;
using CivicConnect.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.Tests.AuditLog;

/// Regression tests for DEF-01 (duplicate request_history rows). RequestService already writes the history
/// row in the same transaction as a status change, so AuditLogEventHandler must NOT also be registered as a
/// listener. Building the container does not open a database connection, so these run without PostgreSQL.
public class AuditLogModuleRegistrationTests
{
    private const string DummyConnection = "Host=localhost;Database=unused;Username=unused;Password=unused";

    [Fact]
    public void AuditLog_module_does_not_register_AuditLogEventHandler()
    {
        using var provider = new ServiceCollection()
            .AddAuditLogModule(DummyConnection)
            .BuildServiceProvider();

        var handlers = provider.GetServices<IDomainEventHandler<RequestStatusChangedEvent>>();

        Assert.DoesNotContain(handlers, h => h is AuditLogEventHandler);
    }

    [Fact]
    public void Status_change_has_exactly_one_listener_the_notification_handler()
    {
        // Same modules as Program.cs (without the message box, so nothing is shown during the test).
        using var provider = new ServiceCollection()
            .AddNotificationsModule()
            .AddAuditLogModule(DummyConnection)
            .BuildServiceProvider();

        var handler = Assert.Single(provider.GetServices<IDomainEventHandler<RequestStatusChangedEvent>>());

        Assert.IsType<NotificationEventHandler>(handler);
    }
}
