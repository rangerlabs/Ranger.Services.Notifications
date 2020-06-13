using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications
{
    class SendNewUserEmailHandler : ICommandHandler<SendNewUserEmail>
    {
        private readonly TenantsHttpClient tenantsHttpClient;
        private readonly ILogger<SendNewUserEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;
        private readonly SendGridOptions sendGridOptions;

        public SendNewUserEmailHandler(TenantsHttpClient tenantsHttpClient, ILogger<SendNewUserEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher, SendGridOptions sendGridOptions)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
            this.sendGridOptions = sendGridOptions;
        }
        public async Task HandleAsync(SendNewUserEmail message, ICorrelationContext context)
        {
            var apiResponse = await tenantsHttpClient.GetTenantByIdAsync<TenantResult>(message.TenantId);
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },
                organization = apiResponse.Result.OrganizationName,
                domain = apiResponse.Result.Domain,
                isUser = Enum.Parse<RolesEnum>(message.Role) == RolesEnum.User,
                role = message.Role,
                confirm = $"https://{sendGridOptions.Host}/confirm-user?domain={apiResponse.Result.Domain}&userId={message.UserId}&token={message.Token}",
                authorizedProjects = message.AuthorizedProjects
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-9b6ca5af2e444c729270908523b8af95", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred sending the necessary email");
                throw new RangerException("An unexpected error occurred sending the necessary email");
            }

            busPublisher.Publish(new SendNewUserEmailSent(), context);
        }
    }
}