using System;
using System.Threading.Tasks;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SendPusherPrivateFrontendNotificationHandler : ICommandHandler<SendPusherDomainUserPredefinedNotification>
    {
        private readonly IPusherNotifier pusherNotifier;
        private readonly ITenantsHttpClient tenantsHttpClient;

        public SendPusherPrivateFrontendNotificationHandler(ITenantsHttpClient tenantsHttpClient, IPusherNotifier pusherNotifier)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherDomainUserPredefinedNotification message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            await pusherNotifier.SendDomainUserPredefinedNotification(context.CorrelationContextId.ToString(), message.BackendEventKey, apiResponse.Result.Domain, message.UserEmail, message.State);
        }
    }
}