using System;
using System.Threading.Tasks;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    public interface IPusherNotifier
    {
        Task SendDomainUserCustomNotification(string id, string eventName, string backendEventName, string domain, string userEmail, OperationsStateEnum state, string resourceId = "");
        Task SendDomainUserPredefinedNotification(string id, string backendEventName, string domain, string userEmail, OperationsStateEnum state);
        Task SendDomainFrontendNotification(string id, string backendEventName, string domain, OperationsStateEnum state);
    }
}