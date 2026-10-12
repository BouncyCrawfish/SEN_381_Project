using CivicConnect.Common;
using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.Notifications
{
    public static class NotificationsModuleExtensions
    {
        public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
        {
            services.AddSingleton<INotificationService, NullNotificationService>();
            return services;
        }

        public static IServiceCollection AddMessageBoxNotification(this IServiceCollection services)
        {
            services.AddSingleton<INotificationService, MessageBoxNotificationService>();
            return services;
        }
    }
}