using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespaceAttribute("notifications")]
    public class SendUserPermissionsUpdatedEmailSent : IEvent
    {
        public SendUserPermissionsUpdatedEmailSent() { }
    }
}