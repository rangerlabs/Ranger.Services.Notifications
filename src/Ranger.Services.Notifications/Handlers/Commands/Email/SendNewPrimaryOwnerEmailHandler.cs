using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers
{
    class SendNewPrimaryOwnerEmailHandler : ICommandHandler<SendNewPrimaryOwnerEmail>
    {
        private readonly ILogger<SendNewPrimaryOwnerEmailHandler> logger;
        private readonly TenantsHttpClient tenantsHttpClient;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendNewPrimaryOwnerEmailHandler(ILogger<SendNewPrimaryOwnerEmailHandler> logger, TenantsHttpClient tenantsHttpClient, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.logger = logger;
            this.tenantsHttpClient = tenantsHttpClient;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendNewPrimaryOwnerEmail message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },
                domain = apiResponse.Result.Domain,
                confirm = $"https://rangerlabs.io/confirm-domain?domain={apiResponse.Result.Domain}&token={message.Token}"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-9e081b2a65554d9d8c95bf40ed944f1b", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send new tenant owner email");
                throw;
            }

            busPublisher.Publish(new SendNewPrimaryOwnerEmailSent(), context);
        }
    }
}