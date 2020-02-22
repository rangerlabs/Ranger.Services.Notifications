using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespaceAttribute("notifications")]
    public class SendPrimaryOwnerTransferCancelledEmailsSent : IEvent
    {
        public SendPrimaryOwnerTransferCancelledEmailsSent() { }
    }
}