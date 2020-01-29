using System.Threading.Tasks;
using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    public class SendPusherDomainUserCustomNotificationHandler : ICommandHandler<SendPusherDomainUserCustomNotification>
    {
        private readonly IPusherNotifier pusherNotifier;

        public SendPusherDomainUserCustomNotificationHandler(IPusherNotifier pusherNotifier)
        {
            this.pusherNotifier = pusherNotifier;
        }

        public async Task HandleAsync(SendPusherDomainUserCustomNotification message, ICorrelationContext context)
        {
            await pusherNotifier.SendDomainUserCustomNotification(context.CorrelationContextId.ToString(), message.EventName, message.Message, message.Domain, message.UserEmail, message.State, message.ResourceId);
        }
    }
}