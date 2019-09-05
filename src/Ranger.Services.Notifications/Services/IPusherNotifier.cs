using System;
using System.Threading.Tasks;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    public interface IPusherNotifier
    {
        Task SendPrivateFrontendNotification(string id, string backendEventName, string domain, string userEmail, OperationsStateEnum state);
        Task SendDomainFrontendNotification(string id, string backendEventName, string domain, OperationsStateEnum state);
    }
}