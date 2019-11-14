using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendPusherDomainUserCustomNotification : ICommand
    {
        public string EventName { get; }
        public string Message { get; }
        public string Domain { get; }
        public string UserEmail { get; }
        public OperationsStateEnum State { get; }

        public SendPusherDomainUserCustomNotification(string eventName, string message, string domain, string userEmail, OperationsStateEnum state)
        {
            this.EventName = eventName;
            this.Message = message;
            this.Domain = domain;
            this.UserEmail = userEmail;
            this.State = state;
        }
    }
}