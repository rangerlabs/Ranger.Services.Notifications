using System;
using System.Threading.Tasks;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SendPusherDomainUserCustomNotificationHandler : ICommandHandler<SendPusherDomainUserCustomNotification>
    {
        private readonly IPusherNotifier pusherNotifier;
        private readonly ITenantsHttpClient tenantsHttpClient;

        public SendPusherDomainUserCustomNotificationHandler(ITenantsHttpClient tenantsHttpClient, IPusherNotifier pusherNotifier)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherDomainUserCustomNotification message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            await pusherNotifier.SendDomainUserCustomNotification(context.CorrelationContextId.ToString(), message.EventName, message.Message, apiResponse.Result.Domain, message.UserEmail, message.State, message.ResourceId);
        }
    }
}