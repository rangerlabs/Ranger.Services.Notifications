using System.Threading.Tasks;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SendPusherPrivateFrontendNotificationHandler : ICommandHandler<SendPusherDomainUserPredefinedNotification>
    {
        private readonly IPusherNotifier pusherNotifier;

        public SendPusherPrivateFrontendNotificationHandler(IPusherNotifier pusherNotifier)
        {
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherDomainUserPredefinedNotification message, ICorrelationContext context)
        {
            await pusherNotifier.SendDomainUserPredefinedNotification(context.CorrelationContextId.ToString(), message.BackendEventKey, message.Domain, message.UserEmail, message.State);
        }
    }
}