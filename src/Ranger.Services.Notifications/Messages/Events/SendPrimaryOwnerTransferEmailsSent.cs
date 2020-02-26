using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespaceAttribute("notifications")]
    public class SendPrimaryOwnerTransferEmailsSent : IEvent
    {
        public SendPrimaryOwnerTransferEmailsSent() { }
    }
}