using System;
using System.Threading.Tasks;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    public interface IPusherNotifier
    {
        Task SendDomainUserCustomNotification(string id, string eventName, string backendEventName, string domain, string userEmail, OperationsStateEnum state, Guid resourceId = default(Guid));
        Task SendDomainUserPredefinedNotification(string id, string backendEventName, string domain, string userEmail, OperationsStateEnum state);
        Task SendDomainFrontendNotification(string id, string backendEventName, string domain, OperationsStateEnum state);
        Task SendDomainCustomNotification(string eventName, string domain, string message);
        Task SendOrganizationDomainUpdatedNotification(string eventName, string domain, string message, string newDomain);
    }
}