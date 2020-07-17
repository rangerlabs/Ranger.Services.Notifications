using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers
{
    class SendContactFormEmailHandler : ICommandHandler<SendContactFormEmail>
    {
        private readonly ILogger<SendResetPasswordEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly SendGridOptions sendGridOptions;

        public SendContactFormEmailHandler(ILogger<SendResetPasswordEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher, SendGridOptions sendGridOptions)
        {
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.sendGridOptions = sendGridOptions;
        }
        public async Task HandleAsync(SendContactFormEmail message, ICorrelationContext context)
        {
            var personalizationData = new
            {
                organization = message.Organization,
                email = message.Email,
                message = message.Message
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress("info@rangerlabs.io"), "d-42f1287584594bc7a18fdac60bba0382", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred sending the necessary emails");
                throw new RangerException("An unexpected error occurred sending the necessary emails");
            }
        }
    }
}