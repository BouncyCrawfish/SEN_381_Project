using CivicConnect.Common;

namespace CivicConnect.Notifications
{
	public sealed class NullNotificationService : INotificationService
	{
		public void Notify(NotificationMessage message) { /* intentionally no-op */ }
	}
}