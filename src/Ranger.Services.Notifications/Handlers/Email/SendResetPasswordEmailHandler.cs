﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.RabbitMQ;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications.Handlers
{
    class SendResetPasswordEmailHandler : ICommandHandler<SendResetPasswordEmail>
    {
        private readonly ILogger<SendResetPasswordEmailHandler> logger;
        private readonly IEmailNotifier emailNotifier;
        private readonly IBusPublisher busPublisher;

        public SendResetPasswordEmailHandler(ILogger<SendResetPasswordEmailHandler> logger, IEmailNotifier emailNotifier, IBusPublisher busPublisher)
        {
            this.logger = logger;
            this.emailNotifier = emailNotifier;
            this.busPublisher = busPublisher;
        }
        public async Task HandleAsync(SendResetPasswordEmail message, ICorrelationContext context)
        {
            var personalizationData = new
            {
                user = new
                {
                    firstname = message.FirstName,
                },
                domain = message.Domain,
                organization = message.Organization,
                reset = $"https://rangerlabs.io/password-reset?domain={message.Domain}&userId={message.UserId}&token={message.Token}"
            };
            try
            {
                await emailNotifier.SendAsync(new EmailAddress(message.Email), "d-fabffc38e89d4de5b89eadf45324ff78", personalizationData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send password reset email.");
                throw;
            }
        }
    }
}