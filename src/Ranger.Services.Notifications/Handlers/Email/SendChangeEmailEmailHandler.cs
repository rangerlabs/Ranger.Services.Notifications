using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications
{
    class SendChangeEmailEmailHandler : ICommandHandler<SendChangeEmailEmail>
    {
        private readonly TenantsHttpClient tenantsHttpClient;
        private readonly ILogger<SendChangeEmailEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendChangeEmailEmailHandler(TenantsHttpClient tenantsHttpClient, ILogger<SendChangeEmailEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendChangeEmailEmail message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            if (apiResponse.IsError)
            {
                throw new Exception("No tenant was found for the provided tenant id");
            }
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },
                organization = message.Organization,
                change = $"https://rangerlabs.io/email-change?={apiResponse.Result.Domain}&userId={message.UserId}&token={message.Token}"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-c693f151003a401383e77819ebb85295", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send change email email.");
                throw;
            }
        }
    }
}