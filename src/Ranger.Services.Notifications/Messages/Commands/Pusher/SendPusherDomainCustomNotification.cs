using System;
using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Operations
{
    [MessageNamespace("notifications")]
    public class SendPusherDomainCustomNotification : ICommand
    {
        public string EventName { get; }
        public string Message { get; }
        public string TenantId { get; }

        public SendPusherDomainCustomNotification(string eventName, string message, string tenantId, string userEmail)
        {
            this.EventName = eventName;
            this.Message = message;
            this.TenantId = tenantId;
        }
    }
}