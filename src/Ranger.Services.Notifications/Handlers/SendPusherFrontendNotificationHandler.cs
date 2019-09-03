using System.Threading.Tasks;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SendPusherUserNotificationHandler : IEventHandler<SendPusherFrontendNotification>
    {
        private readonly IPusherNotifier pusherNotifier;

        public SendPusherUserNotificationHandler(IPusherNotifier pusherNotifier)
        {
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherFrontendNotification message, ICorrelationContext context)
        {
            await pusherNotifier.SendUserNotification(context.CorrelationContextId.ToString(), message.BackendEventName, context.Domain, context.UserEmail, message.State);
        }
    }
}