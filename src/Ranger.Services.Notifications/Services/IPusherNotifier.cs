using System;
using System.Threading.Tasks;

namespace Ranger.Services.Notifications
{
    public interface IPusherNotifier
    {
        Task SendUserNotification(string id, string backendEventName, string domain, string userEmail, OperationsStateEnum state);
    }
}