using System.Threading.Tasks;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SubscriptionUpdatedHandler : IEventHandler<SubscriptionUpdated>
    {
        private readonly IPusherNotifier pusherNotifier;
        private readonly TenantsHttpClient tenantsHttpClient;

        public SubscriptionUpdatedHandler(TenantsHttpClient tenantsHttpClient, IPusherNotifier pusherNotifier)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SubscriptionUpdated message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            await pusherNotifier.SendDomainCustomNotification("subscription-changed", "Your subscription has changed. Retrieving new subscription details", apiResponse.Result.Domain);
        }
    }
}