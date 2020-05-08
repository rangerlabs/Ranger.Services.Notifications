using System;
using System.Threading.Tasks;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SendPusherDomainFrontendNotificationHandler : ICommandHandler<SendPusherDomainFrontendNotification>
    {
        private readonly IPusherNotifier pusherNotifier;
        private readonly TenantsHttpClient tenantsHttpClient;

        public SendPusherDomainFrontendNotificationHandler(TenantsHttpClient tenantsHttpClient, IPusherNotifier pusherNotifier)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherDomainFrontendNotification message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            await pusherNotifier.SendDomainFrontendNotification(context.CorrelationContextId.ToString(), message.BackendEventKey, apiResponse.Result.Domain, message.State);
        }
    }
}