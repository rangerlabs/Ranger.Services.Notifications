using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendChangeEmailEmail : ICommand
    {
        public SendChangeEmailEmail(string firstName, string email, string tenantId, string token)
        {
            this.FirstName = firstName;
            this.Email = email;
            this.TenantId = tenantId;
            this.Token = token;

        }
        public string FirstName { get; }
        public string Email { get; }
        public string TenantId { get; }
        public string Token { get; }
    }
}