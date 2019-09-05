
using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendPusherDomainFrontendNotification : ICommand
    {
        public string BackendEventKey { get; }
        public string Domain { get; }
        public OperationsStateEnum State { get; }

        public SendPusherDomainFrontendNotification(string backendEventKey, string domain, OperationsStateEnum state)
        {
            this.BackendEventKey = backendEventKey;
            this.Domain = domain;
            this.State = state;
        }
    }
}