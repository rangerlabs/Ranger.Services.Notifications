using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers
{
    class SendPrimaryOwnerTransferEmailsHandler : ICommandHandler<SendPrimaryOwnerTransferEmails>
    {
        private readonly ILogger<SendPrimaryOwnerTransferEmailsHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendPrimaryOwnerTransferEmailsHandler(ILogger<SendPrimaryOwnerTransferEmailsHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendPrimaryOwnerTransferEmails message, ICorrelationContext context)
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
                organization = message.OrganizationName,
                acceptLink = $"https://{message.Domain.ToLowerInvariant()}.rangerlabs.io/transfer-ownership?correlationId={message.CorrelationId}&token={message.Token}&response={TransferPrimaryOwnershipResultEnum.Accept}",
                rejectLink = $"https://{message.Domain.ToLowerInvariant()}.rangerlabs.io/transfer-ownership?correlationId={message.CorrelationId}&response={TransferPrimaryOwnershipResultEnum.Reject}"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.TransferEmail), "d-a499a694b13e4efdaf651d0de34cb295", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send primary ownership transfer email.");
                throw new RangerException("Failed to send primary ownership transfer email.");
            }

            busPublisher.Publish(new SendPrimaryOwnerTransferEmailsSent(), context);
        }
    }
}