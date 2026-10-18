using CivicConnect.Common;

namespace CivicConnect.Notifications
{
    /// Turns a status-change event into a user-facing notification.
    public sealed class NotificationEventHandler : IDomainEventHandler<RequestStatusChangedEvent>
    {
        private readonly INotificationService _notifications;

        public NotificationEventHandler(INotificationService notifications)
        {
            _notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));
        }

        // Deliberately NOT async: the tests expect a null event or a failing
        // notification channel to throw immediately, not inside the returned Task.
        public Task HandleAsync(RequestStatusChangedEvent evt, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(evt);

            var oldStatus = string.IsNullOrWhiteSpace(evt.OldStatus) ? "New" : evt.OldStatus;

            _notifications.Notify(new NotificationMessage(
                "Request status updated",
                $"Request #{evt.RequestId} changed from {oldStatus} to {evt.NewStatus}.",
                NotificationSeverity.Info));

            return Task.CompletedTask;
        }
    }
}