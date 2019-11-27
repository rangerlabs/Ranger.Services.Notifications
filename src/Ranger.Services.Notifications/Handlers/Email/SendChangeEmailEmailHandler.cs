using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers
{
    class SendChangeEmailEmailHandler : ICommandHandler<SendChangeEmailEmail>
    {
        private readonly ILogger<SendChangeEmailEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendChangeEmailEmailHandler(ILogger<SendChangeEmailEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendChangeEmailEmail message, ICorrelationContext context)
        {
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },
                organization = message.Organization,
                change = $"https://rangerlabs.io/email-change?domain={message.Domain}&userId={message.UserId}&token={message.Token}"
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