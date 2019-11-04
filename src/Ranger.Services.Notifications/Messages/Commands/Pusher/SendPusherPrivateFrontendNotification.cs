using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendPusherPrivateFrontendNotification : ICommand
    {
        public string BackendEventKey { get; }
        public string Domain { get; }
        public string UserEmail { get; }
        public OperationsStateEnum State { get; }

        public SendPusherPrivateFrontendNotification(string backendEventKey, string domain, string userEmail, OperationsStateEnum state)
        {
            this.BackendEventKey = backendEventKey;
            this.Domain = domain;
            this.UserEmail = userEmail;
            this.State = state;
        }
    }
}