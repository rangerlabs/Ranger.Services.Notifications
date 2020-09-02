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
    class SendResetPasswordEmailHandler : ICommandHandler<SendResetPasswordEmail>
    {
        private readonly TenantsHttpClient tenantsHttpClient;
        private readonly ILogger<SendResetPasswordEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;
        private readonly SendGridOptions sendGridOptions;

        public SendResetPasswordEmailHandler(TenantsHttpClient tenantsHttpClient, ILogger<SendResetPasswordEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher, SendGridOptions sendGridOptions)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
            this.sendGridOptions = sendGridOptions;
        }
        public async Task HandleAsync(SendResetPasswordEmail message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },
                domain = apiResponse.Result.Domain,
                organization = apiResponse.Result.OrganizationName,
                reset = $"https://{sendGridOptions.Host}/password-reset?domain={apiResponse.Result.Domain}&userId={message.UserId}&token={message.Token}"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-fabffc38e89d4de5b89eadf45324ff78", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred sending the necessary emails");
                throw new RangerException("An unexpected error occurred sending the necessary emails");
            }
        }
    }
}