namespace CivicConnect.Common
{
    /// Abstraction for notifying the user. Implementations decide how to deliver the message.
    public interface INotificationService
    {
        void Notify(NotificationMessage message);
    }
}