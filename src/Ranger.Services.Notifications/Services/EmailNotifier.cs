using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications
{
    public class EmailNotifier : IEmailNotifier
    {
        private readonly ILogger<EmailNotifier> logger;
        private EmailAddress from = new EmailAddress("notifications@rangerlabs.io");
        private string apiKey = "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";

        public EmailNotifier(IConfiguration configuration, ILogger<EmailNotifier> logger)
        {
            string apiKey = "";
            try
            {
                apiKey = configuration["sendGrid:apiKey"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occurred reading the SendGrid api key from configuration. Verify the 'sendGrid:apiKey' configuration section is set.");
            }
            this.apiKey = apiKey;
            this.logger = logger;
        }

        public async Task SendAsync(EmailAddress to, string templateId, object templateData)
        {
            if (to is null)
            {
                throw new System.ArgumentException($"{nameof(to)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(templateId))
            {
                throw new System.ArgumentException($"{nameof(templateId)} cannot be null or whitespace.");
            }

            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = this.from,
                TemplateId = templateId,
            };
            msg.AddTo(to);
            msg.SetTemplateData(templateData);

            var response = await client.SendEmailAsync(msg);
            if (!IsSuccessfulStatusCode(response))
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email to '{to.Email}'. SendGrid returned status code '{response.StatusCode}' with body '{body}'.");
            }
        }

        private bool IsSuccessfulStatusCode(Response response)
        {
            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
        }
    }
}