using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications
{
    class SendNewUserEmailHandler : ICommandHandler<SendNewUserEmail>
    {
        private readonly ILogger<SendNewUserEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendNewUserEmailHandler(ILogger<SendNewUserEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendNewUserEmail message, ICorrelationContext context)
        {
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },
                organization = message.Organization,
                isUser = Enum.Parse<RolesEnum>(message.Role) == RolesEnum.User,
                role = message.Role,
                confirm = $"https://rangerlabs.io/confirm-user?domain={message.Domain}&userId={message.UserId}&token={message.Token}",
                authorizedProjects = message.AuthorizedProjects
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-9b6ca5af2e444c729270908523b8af95", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send new user email.");
                throw;
            }

            busPublisher.Publish(new SendNewUserEmailSent(), context);
        }
    }
}