using CivicConnect.Common;
using CivicConnect.Notifications;

namespace CivicConnect.Tests.Notifications;

public class NotificationServicesTests
{
    [Fact]
    public void A_message_defaults_to_Info_severity()
    {
        var message = new NotificationMessage("Title", "Body");

        Assert.Equal(NotificationSeverity.Info, message.Severity);
        Assert.Equal("Title", message.Title);
        Assert.Equal("Body", message.Body);
    }

    [Theory]
    [InlineData(NotificationSeverity.Info)]
    [InlineData(NotificationSeverity.Warning)]
    [InlineData(NotificationSeverity.Error)]
    public void A_message_keeps_the_severity_it_was_given(NotificationSeverity severity)
    {
        Assert.Equal(severity, new NotificationMessage("T", "B", severity).Severity);
    }

    [Fact]
    public void A_null_title_or_body_becomes_empty_text()
    {
        var message = new NotificationMessage(null!, null!);

        Assert.Equal(string.Empty, message.Title);
        Assert.Equal(string.Empty, message.Body);
    }

    [Fact]
    public void The_null_service_accepts_a_message_and_does_nothing()
    {
        INotificationService service = new NullNotificationService();

        Assert.Null(Record.Exception(() => service.Notify(new NotificationMessage("T", "B"))));
    }

    [Fact]
    public void The_null_service_accepts_a_null_message()
    {
        INotificationService service = new NullNotificationService();

        Assert.Null(Record.Exception(() => service.Notify(null!)));
    }

    [Fact]
    public void The_message_box_service_ignores_a_null_message_without_showing_anything()
    {
        // The null check returns before MessageBox.Show, so this is safe to run without a UI.
        INotificationService service = new MessageBoxNotificationService();

        Assert.Null(Record.Exception(() => service.Notify(null!)));
    }
}
