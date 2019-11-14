using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespaceAttribute("notifications")]
    public class SendNewUserEmailRejected : IRejectedEvent
    {
        public string Reason { get; }
        public string Code { get; }

        public SendNewUserEmailRejected(string message, string code)
        {
            this.Reason = message;
            this.Code = code;
        }
    }
}