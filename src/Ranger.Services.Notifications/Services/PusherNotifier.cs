using System;
using System.Linq;
using System.Threading.Tasks;
using PusherServer;
using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    public class PusherNotifier : IPusherNotifier
    {
        private readonly IPusher pusher;
        private readonly NotificationsDbContext notificationsDbContext;

        public PusherNotifier(IPusher pusher, NotificationsDbContext notificationsDbContext)
        {
            this.pusher = pusher;
            this.notificationsDbContext = notificationsDbContext;
        }

        public async Task SendPrivateFrontendNotification(string id, string backendEventName, string domain, string userEmail, OperationsStateEnum state)
        {
            var userNotification = notificationsDbContext.FrontendNotifications.FirstOrDefault(un => un.BackendEventKey == backendEventName && un.OperationsState == state);
            if (userNotification != null)
            {
                var result = await pusher.TriggerAsync(
                    $"private-{domain}-{userEmail}",
                    userNotification.PusherEventName,
                    new { correlationId = id, message = userNotification.Text, status = state }
                );
            }
        }

        public async Task SendDomainFrontendNotification(string id, string backendEventName, string domain, OperationsStateEnum state)
        {
            var userNotification = notificationsDbContext.FrontendNotifications.FirstOrDefault(un => un.BackendEventKey == backendEventName && un.OperationsState == state);
            if (userNotification != null)
            {
                var result = await pusher.TriggerAsync(
                    $"ranger-labs-{domain}",
                    userNotification.PusherEventName,
                    new { correlationId = id, message = userNotification.Text, status = state }
                );
            }
        }
    }
}