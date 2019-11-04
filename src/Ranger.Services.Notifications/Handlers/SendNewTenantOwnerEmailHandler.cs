using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers
{
    class SendNewTenantOwnerEmailHandler : ICommandHandler<SendNewTenantOwnerEmail>
    {
        private readonly ILogger<SendNewTenantOwnerEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendNewTenantOwnerEmailHandler(ILogger<SendNewTenantOwnerEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendNewTenantOwnerEmail message, ICorrelationContext context)
        {
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },
                domain = message.Domain,
                confirm = $"https://rangerlabs.io/confirmdomain?domain={message.Domain}&registrationKey={message.RegistrationCode}"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-9e081b2a65554d9d8c95bf40ed944f1b", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send new tenant owner email.");
                throw;
            }

            busPublisher.Publish(new SendNewTenantOwnerEmailSent(), context);
        }
    }
}