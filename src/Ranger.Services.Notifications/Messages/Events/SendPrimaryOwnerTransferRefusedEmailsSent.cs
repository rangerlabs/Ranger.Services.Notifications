using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespaceAttribute("notifications")]
    public class SendPrimaryOwnerTransferRefusedEmailsSent : IEvent
    {
        public SendPrimaryOwnerTransferRefusedEmailsSent() { }
    }
}