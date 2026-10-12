namespace CivicConnect.Common
{
    public enum NotificationSeverity
    {
        Info,
        Warning,
        Error
    }

    public sealed class NotificationMessage
    {
        public string Title { get; init; }
        public string Body { get; init; }
        public NotificationSeverity Severity { get; init; } = NotificationSeverity.Info;

        public NotificationMessage(string title, string body, NotificationSeverity severity = NotificationSeverity.Info)
        {
            Title = title ?? string.Empty;
            Body = body ?? string.Empty;
            Severity = severity;
        }
    }
}