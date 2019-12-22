using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespaceAttribute("notifications")]
    public class SendNewPrimaryOwnerEmailSent : IEvent
    {
        public SendNewPrimaryOwnerEmailSent() { }
    }
}