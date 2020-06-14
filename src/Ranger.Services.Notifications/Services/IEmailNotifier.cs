using System.Collections.Generic;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace Ranger.Services.Notifications
{
    public interface IEmailNotifier
    {
        Task SendAsync(EmailAddress to, string templateId, object templateData);
        Task SendAsync(IEnumerable<EmailAddress> to, string templateId, object templateData);
    }
}