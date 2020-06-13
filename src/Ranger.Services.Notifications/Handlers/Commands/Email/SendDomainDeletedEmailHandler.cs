using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers
{
    class SendDomainDeletedEmailHandler : ICommandHandler<SendDomainDeletedEmail>
    {
        private readonly ILogger<SendDomainDeletedEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendDomainDeletedEmailHandler(ILogger<SendDomainDeletedEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendDomainDeletedEmail message, ICorrelationContext context)
        {
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },

                organization = message.OrganizationName,
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-31a92e2db2bd47eab8e4dbe1d2ec092f", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred sending the necessary email");
                throw new RangerException("An unexpected error occurred sending the necessary email");
            }

            busPublisher.Publish(new SendDomainDeletedEmailSent(), context);
        }
    }
}