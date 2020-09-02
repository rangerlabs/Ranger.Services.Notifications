using System;
using System.Threading.Tasks;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using Ranger.Services.Operations;

namespace Ranger.Services.Notifications
{
    public class SendPusherDomainCustomNotificationHandler : ICommandHandler<SendPusherDomainCustomNotification>
    {
        private readonly IPusherNotifier pusherNotifier;
        private readonly ITenantsHttpClient tenantsHttpClient;

        public SendPusherDomainCustomNotificationHandler(ITenantsHttpClient tenantsHttpClient, IPusherNotifier pusherNotifier)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherDomainCustomNotification message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            await pusherNotifier.SendDomainCustomNotification(message.EventName, message.Message, apiResponse.Result.Domain);
        }
    }
}