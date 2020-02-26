using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespaceAttribute("notifications")]
    public class SendPrimaryOwnerTransferAcceptedEmailsSent : IEvent
    {
        public SendPrimaryOwnerTransferAcceptedEmailsSent() { }
    }
}