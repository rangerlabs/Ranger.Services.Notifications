using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespaceAttribute("notifications")]
    public class SendNewUserEmailSent : IEvent
    {
        public SendNewUserEmailSent() { }
    }
}