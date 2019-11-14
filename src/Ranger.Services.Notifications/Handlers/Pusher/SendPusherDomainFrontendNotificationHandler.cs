using System.Threading.Tasks;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SendPusherDomainFrontendNotificationHandler : ICommandHandler<SendPusherDomainFrontendNotification>
    {
        private readonly IPusherNotifier pusherNotifier;

        public SendPusherDomainFrontendNotificationHandler(IPusherNotifier pusherNotifier)
        {
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherDomainFrontendNotification message, ICorrelationContext context)
        {
            await pusherNotifier.SendDomainFrontendNotification(context.CorrelationContextId.ToString(), message.BackendEventKey, message.Domain, message.State);
        }
    }
}