using System;
using System.Threading.Tasks;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using Ranger.Services.Operations;

namespace Ranger.Services.Notifications
{
    public class SendPusherOrganizationDomainUpdatedNotificationHandler : ICommandHandler<SendPusherOrganizationDomainUpdatedNotification>
    {
        private readonly IPusherNotifier pusherNotifier;
        private readonly TenantsHttpClient tenantsHttpClient;

        public SendPusherOrganizationDomainUpdatedNotificationHandler(TenantsHttpClient tenantsHttpClient, IPusherNotifier pusherNotifier)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherOrganizationDomainUpdatedNotification message, ICorrelationContext context)
        {
            await pusherNotifier.SendOrganizationDomainUpdatedNotification(message.EventName, message.Message, message.OldDomain, message.NewDomain);
        }
    }
}