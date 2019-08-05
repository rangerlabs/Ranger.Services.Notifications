using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications.Handlers {
    public class SendNewTenantOwnerEmailHandler : ICommandHandler<SendNewTenantOwnerEmail> {
        public SendNewTenantOwnerEmailHandler () { }
        var apiKey = Environment.GetEnvironmentVariable ("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");
        var client = new SendGridClient (apiKey);
        var from = new EmailAddress ("test@example.com", "Example User");
        var subject = "Sending with SendGrid is Fun";
        var to = new EmailAddress ("test@example.com", "Example User");
        var plainTextContent = "and easy to do anywhere, even with C#";
        var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        var msg = MailHelper.CreateSingleEmail (from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync (msg);

    }
}