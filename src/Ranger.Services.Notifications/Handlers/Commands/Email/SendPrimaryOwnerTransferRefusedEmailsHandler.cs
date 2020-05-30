using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers.Email
{
    public class SendPrimaryOwnerTransferRefusedEmailsHandler : ICommandHandler<SendPrimaryOwnerTransferRefusedEmails>
    {
        private readonly ILogger<SendPrimaryOwnerTransferRefusedEmails> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;
        private readonly SendGridOptions sendGridOptions;
        private readonly TenantsHttpClient tenantsHttpClient;

        public SendPrimaryOwnerTransferRefusedEmailsHandler(TenantsHttpClient tenantsHttpClient, ILogger<SendPrimaryOwnerTransferRefusedEmails> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher, SendGridOptions sendGridOptions)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
            this.sendGridOptions = sendGridOptions;
        }
        public async Task HandleAsync(SendPrimaryOwnerTransferRefusedEmails message, ICorrelationContext context)
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
                loginLink = $"https://{apiResponse.Result.Domain.ToLowerInvariant()}.{sendGridOptions.Host}/login"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.TransferEmail), "d-ac83f9acea5e404ea96066afc9c079bb", personalizationData);
                await emailNotifier.SendAsync(new EmailAddress(message.OwnerEmail), "d-f58ce5e104294c0d83431d84fc8ae693", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send primary ownership transfer refused emails");
            }

            busPublisher.Publish(new SendPrimaryOwnerTransferRefusedEmailsSent(), context);
        }
    }
}