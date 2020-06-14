using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers.Commands.Email
{
    public class SendTenantDomainUpdatedEmailsHandler : ICommandHandler<SendTenantDomainUpdatedEmails>
    {
        private readonly TenantsHttpClient tenantsHttpClient;
        private readonly IdentityHttpClient identityHttpClient;
        private readonly IEmailNotifier emailNotifier;
        private readonly SendGridOptions sendGridOptions;
        private readonly ILogger<SendTenantDomainUpdatedEmailsHandler> logger;

        public SendTenantDomainUpdatedEmailsHandler(TenantsHttpClient tenantsHttpClient, IdentityHttpClient identityHttpClient, IEmailNotifier emailNotifier, SendGridOptions sendGridOptions, ILogger<SendTenantDomainUpdatedEmailsHandler> logger)
        {
            this.tenantsHttpClient = tenantsHttpClient;
            this.identityHttpClient = identityHttpClient;
            this.emailNotifier = emailNotifier;
            this.sendGridOptions = sendGridOptions;
            this.logger = logger;
        }

        public async Task HandleAsync(SendTenantDomainUpdatedEmails message, ICorrelationContext context)
        {
            var usersApiResponse = await identityHttpClient.GetAllUsersAsync<IEnumerable<User>>(message.TenantId);
            if (usersApiResponse.IsError)
            {
                logger.LogError("An unexpected error occurred sending the necessary emails");
                throw new RangerException("An unexpected error occurred sending the necessary emails");
            }
            var personalizationData = new
            {
                domain = message.Domain,
                fullDomain = $"https://{message.Domain}.{sendGridOptions.Host}"
            };
            try
            {
                await emailNotifier.SendAsync(usersApiResponse.Result.Select(u => new EmailAddress(u.Email)), "d-60cd9ea8d43f46e09ac78605ffcf6a84", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred sending the necessary emails");
                throw new RangerException("An unexpected error occurred sending the necessary emails");
            }
        }
    }
}