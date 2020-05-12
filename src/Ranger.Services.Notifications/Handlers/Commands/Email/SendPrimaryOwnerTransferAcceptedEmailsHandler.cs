using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers.Email
{
    public class SendPrimaryOwnerTransferAcceptedEmailsHandler : ICommandHandler<SendPrimaryOwnerTransferAcceptedEmails>
    {
        private readonly ILogger<SendPrimaryOwnerTransferAcceptedEmails> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;
        private readonly TenantsHttpClient tenantsHttpClient;

        public SendPrimaryOwnerTransferAcceptedEmailsHandler(TenantsHttpClient tenantsHttpClient, ILogger<SendPrimaryOwnerTransferAcceptedEmails> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendPrimaryOwnerTransferAcceptedEmails message, ICorrelationContext context)
        {

            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
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
                organization = apiResponse.Result.OrganizationName,
                domain = apiResponse.Result.Domain,
                loginLink = $"https://{apiResponse.Result.Domain.ToLowerInvariant()}.rangerlabs.io/login",
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.TransferEmail), "d-c85209b2d496448fbfeace30e8be1917", personalizationData);
                await emailNotifier.SendAsync(new EmailAddress(message.OwnerEmail), "d-471b1209218a4c36ad990f24ea52f9e2", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send primary ownership transfer accepted emails");
            }

            busPublisher.Publish(new SendPrimaryOwnerTransferAcceptedEmailsSent(), context);
        }
    }
}