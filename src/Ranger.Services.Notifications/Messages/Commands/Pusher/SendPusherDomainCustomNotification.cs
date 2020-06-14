using System;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendPusherDomainCustomNotification : ICommand
    {
        public string EventName { get; }
        public string Message { get; }
        public string TenantId { get; }

        public SendPusherDomainCustomNotification(string eventName, string message, string tenantId)
        {
            this.EventName = eventName;
            this.Message = message;
            this.TenantId = tenantId;
        }
    }
}