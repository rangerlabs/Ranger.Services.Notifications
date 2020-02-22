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
                domain = message.Domain,
                transferEmail = message.TransferEmail,
                ownerEmail = message.OwnerEmail,
                acceptLink = $"https://{message.Domain.ToLowerInvariant()}.rangerlabs.io/transfer-ownership?correlationId={message.CorrelationId}&token={message.Token}&response={TransferPrimaryOwnershipResultEnum.Accept}",
                rejectLink = $"https://{message.Domain.ToLowerInvariant()}.rangerlabs.io/transfer-ownership?correlationId={message.CorrelationId}&response={TransferPrimaryOwnershipResultEnum.Reject}",
                cancelLink = $"https://{message.Domain.ToLowerInvariant()}.rangerlabs.io/cancel-ownership-transfer?correlationId={message.CorrelationId}"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.TransferEmail), "d-a499a694b13e4efdaf651d0de34cb295", personalizationData);
                await emailNotifier.SendAsync(new EmailAddress(message.OwnerEmail), "d-b9be5636359c4c9c8804a708b5029913", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send primary ownership transfer emails.");
                throw new RangerException("Failed to send primary ownership transfer emails.");
            }

            busPublisher.Publish(new SendPrimaryOwnerTransferEmailsSent(), context);
        }
    }
}