using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using Ranger.RabbitMQ.BusPublisher;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers
{
    class SendNewPrimaryOwnerEmailHandler : ICommandHandler<SendNewPrimaryOwnerEmail>
    {
        private readonly ILogger<SendNewPrimaryOwnerEmailHandler> logger;
        private readonly ITenantsHttpClient tenantsHttpClient;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;
        private readonly SendGridOptions sendGridOptions;

        public SendNewPrimaryOwnerEmailHandler(ILogger<SendNewPrimaryOwnerEmailHandler> logger, ITenantsHttpClient tenantsHttpClient, IEmailNotifier emailNotifier, IBusPublisher busPublisher, SendGridOptions sendGridOptions)
        {
            this.logger = logger;
            this.tenantsHttpClient = tenantsHttpClient;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
            this.sendGridOptions = sendGridOptions;
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
                confirm = $"https://{sendGridOptions.Host}/confirm-domain?domain={apiResponse.Result.Domain}&token={message.Token}"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-9e081b2a65554d9d8c95bf40ed944f1b", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred sending the necessary email");
                throw new RangerException("An unexpected error occurred sending the necessary email");
            }

            busPublisher.Publish(new SendNewPrimaryOwnerEmailSent(), context);
        }
    }
}