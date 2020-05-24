using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendPusherDomainUserPredefinedNotification : ICommand
    {
        public string BackendEventKey { get; }
        public string TenantId { get; }
        public string UserEmail { get; }
        public OperationsStateEnum State { get; }

        public SendPusherDomainUserPredefinedNotification(string backendEventKey, string tenantId, string userEmail, OperationsStateEnum state = OperationsStateEnum.Completed)
        {
            this.BackendEventKey = backendEventKey;
            this.TenantId = tenantId;
            this.UserEmail = userEmail;
            this.State = state;
        }
    }
}