using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("subscriptions")]
    public class SubscriptionUpdated : IEvent
    {
        public string TenantId { get; }

        public SubscriptionUpdated(string tenantId)
        {
            this.TenantId = tenantId;
        }
    }
}