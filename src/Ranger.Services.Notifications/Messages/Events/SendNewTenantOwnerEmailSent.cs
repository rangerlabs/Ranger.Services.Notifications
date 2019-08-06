using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications {
    [MessageNamespaceAttribute ("notifications")]
    public class SendNewTenantOwnerEmailSent : IEvent {
        public SendNewTenantOwnerEmailSent () { }
    }
}