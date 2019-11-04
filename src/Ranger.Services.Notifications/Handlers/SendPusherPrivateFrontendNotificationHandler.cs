using System.Threading.Tasks;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SendPusherPrivateFrontendNotificationHandler : ICommandHandler<SendPusherPrivateFrontendNotification>
    {
        private readonly IPusherNotifier pusherNotifier;

        public SendPusherPrivateFrontendNotificationHandler(IPusherNotifier pusherNotifier)
        {
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherPrivateFrontendNotification message, ICorrelationContext context)
        {
            await pusherNotifier.SendPrivateFrontendNotification(context.CorrelationContextId.ToString(), message.BackendEventKey, message.Domain, message.UserEmail, message.State);
        }
    }
}