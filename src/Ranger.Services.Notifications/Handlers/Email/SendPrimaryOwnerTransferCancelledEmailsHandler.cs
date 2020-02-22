using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers.Email
{
    public class SendPrimaryOwnerTransferCancelledEmailsHandler : ICommandHandler<SendPrimaryOwnerTransferCancelledEmails>
    {
        private readonly ILogger<SendPrimaryOwnerTransferCancelledEmails> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendPrimaryOwnerTransferCancelledEmailsHandler(ILogger<SendPrimaryOwnerTransferCancelledEmails> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendPrimaryOwnerTransferCancelledEmails message, ICorrelationContext context)
        {
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.TransferFirstName,
                },
                owner = new
                {
                    firstname = message.OwnerFirstName,
                    lastname = message.OwnerLastName
                },
                transferEmail = message.TransferEmail,
                ownerEmail = message.OwnerEmail,
                organization = message.OrganizationName,
                domain = message.Domain,
                loginLink = $"https://{message.Domain.ToLowerInvariant()}.rangerlabs.io/login",
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.TransferEmail), "d-a89c831232814f4a97b304d1dd4c39cf", personalizationData);
                await emailNotifier.SendAsync(new EmailAddress(message.OwnerEmail), "d-76b64c64d36a4283b3bc9151aec4ba1c", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send primary ownership transfer cancelled emails.");
            }

            busPublisher.Publish(new SendPrimaryOwnerTransferCancelledEmailsSent(), context);
        }
    }
}