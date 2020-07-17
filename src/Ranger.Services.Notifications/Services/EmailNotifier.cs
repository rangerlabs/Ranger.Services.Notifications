using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications
{
    public class EmailNotifier : IEmailNotifier
    {
        private readonly ILogger<EmailNotifier> logger;
        private EmailAddress from = new EmailAddress("notifications@rangerlabs.io", "Ranger Notifications");
        private string apiKey = "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";

        public EmailNotifier(SendGridOptions sendGridOptions, ILogger<EmailNotifier> logger)
        {
            string apiKey = "";
            try
            {
                apiKey = sendGridOptions.ApiKey;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occurred reading the SendGrid api key from configuration. Verify the 'sendGrid:apiKey' configuration section is set");
            }
            this.apiKey = apiKey;
            this.logger = logger;
        }

        public async Task SendAsync(EmailAddress to, string templateId, object templateData, EmailAddress from = null)
        {
            if (to is null)
            {
                throw new ArgumentException($"{nameof(to)} cannot be null");
            }

            if (string.IsNullOrWhiteSpace(templateId))
            {
                throw new ArgumentException($"{nameof(templateId)} cannot be null or whitespace");
            }

            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = from ?? this.from,
                TemplateId = templateId,
            };
            msg.AddTo(to);
            msg.SetTemplateData(templateData);

            var response = await client.SendEmailAsync(msg);
            if (!IsSuccessfulStatusCode(response))
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email to '{to.Email}'. SendGrid returned status code '{response.StatusCode}' with body '{body}'");
            }
        }

        public async Task SendAsync(IEnumerable<EmailAddress> to, string templateId, object templateData, EmailAddress from = null)
        {
            if (to is default(IEnumerable<EmailAddress>))
            {
                throw new ArgumentException($"{nameof(to)} cannot be default");
            }
            if (string.IsNullOrWhiteSpace(templateId))
            {
                throw new ArgumentException($"{nameof(templateId)} cannot be null or whitespace");
            }

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = from ?? this.from,
                TemplateId = templateId,
            };
            foreach (var email in to)
            {
                msg.AddTo(email);
            }
            msg.SetTemplateData(templateData);

            var response = await client.SendEmailAsync(msg);
            if (!IsSuccessfulStatusCode(response))
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send bulk email. SendGrid returned status code '{response.StatusCode}' with body '{body}'");
            }
        }



        private bool IsSuccessfulStatusCode(Response response)
        {
            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
        }
    }
}