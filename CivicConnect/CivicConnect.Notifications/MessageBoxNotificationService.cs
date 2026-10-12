using CivicConnect.Common;
using System.Windows.Forms;

namespace CivicConnect.Notifications
{
    public sealed class MessageBoxNotificationService : INotificationService
    {
        public void Notify(NotificationMessage message)
        {
            if (message is null) return;

            var icon = message.Severity switch
            {
                NotificationSeverity.Warning => MessageBoxIcon.Warning,
                NotificationSeverity.Error => MessageBoxIcon.Error,
                _ => MessageBoxIcon.Information
            };

            MessageBox.Show(message.Body, message.Title, MessageBoxButtons.OK, icon);
        }
    }
}