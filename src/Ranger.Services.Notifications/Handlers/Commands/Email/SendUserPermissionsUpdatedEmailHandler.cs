using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using Ranger.RabbitMQ.BusPublisher;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications
{
    class SendUserPermissionsUpdatedHandler : ICommandHandler<SendUserPermissionsUpdatedEmail>
    {
        private readonly ITenantsHttpClient tenantsHttpClient;
        private readonly ILogger<SendUserPermissionsUpdatedHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendUserPermissionsUpdatedHandler(ITenantsHttpClient tenantsHttpClient, ILogger<SendUserPermissionsUpdatedHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendUserPermissionsUpdatedEmail message, ICorrelationContext context)
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
                isUser = Enum.Parse<RolesEnum>(message.Role) == RolesEnum.User,
                role = message.Role,
                authorizedProjects = message.AuthorizedProjects
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-872bcb0e06e9487bbe492009f711ce76", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred sending the necessary emails");
                throw new RangerException("An unexpected error occurred sending the necessary emails");
            }

            busPublisher.Publish(new SendUserPermissionsUpdatedEmailSent(), context);
        }
    }
}