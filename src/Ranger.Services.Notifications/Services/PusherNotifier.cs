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

        public async Task SendDomainUserCustomNotification(string id, string eventName, string message, string domain, string userEmail, OperationsStateEnum state, Guid resourceId = default(Guid))
        {
            await pusher.TriggerAsync(
                $"private-{domain}-{userEmail}",
                eventName,
                new { correlationId = id, message = message, status = state, resourceId = resourceId.ToString() }
            );
        }

        public async Task SendDomainUserPredefinedNotification(string id, string backendEventName, string domain, string userEmail, OperationsStateEnum state)
        {
            var notifications = notificationsDbContext.FrontendNotifications.Where(_ => _.BackendEventKey == backendEventName).ToList();
            var notification = notifications.Count() > 1 ? notifications.FirstOrDefault(_ => _.OperationsState == state) : notifications[0];
            if (notification != null)
            {
                await pusher.TriggerAsync(
                    $"private-{domain}-{userEmail}",
                    notification.PusherEventName,
                    new { correlationId = id, message = notification.Text, status = state }
                );
            }
        }

        public async Task SendDomainFrontendNotification(string id, string backendEventName, string domain, OperationsStateEnum state)
        {
            var userNotification = notificationsDbContext.FrontendNotifications.FirstOrDefault(un => un.BackendEventKey == backendEventName && un.OperationsState == state);
            if (userNotification != null)
            {
                await pusher.TriggerAsync(
                    $"ranger-labs-{domain}",
                    userNotification.PusherEventName,
                    new { correlationId = id, message = userNotification.Text, status = state }
               );
            }
        }

        public async Task SendDomainCustomNotification(string eventName, string message, string domain)
        {
            await pusher.TriggerAsync(
                $"private-{domain}",
                eventName,
                new { message = message }
            );
        }

        public async Task SendOrganizationDomainUpdatedNotification(string eventName, string domain, string message, string newDomain)
        {
            await pusher.TriggerAsync(
                $"private-{domain}",
                eventName,
                new { message = message, newDomain = newDomain }
            );

        }
    }
}