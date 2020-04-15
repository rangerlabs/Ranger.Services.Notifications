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
            if (apiResponse.IsError)
            {
                throw new Exception("No tenant was found for the provided tenant id");
            }
            await pusherNotifier.SendDomainFrontendNotification(context.CorrelationContextId.ToString(), message.BackendEventKey, apiResponse.Result.Domain, message.State);
        }
    }
}