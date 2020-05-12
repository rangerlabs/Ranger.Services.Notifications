using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendResetPasswordEmail : ICommand
    {
        public SendResetPasswordEmail(string firstName, string email, string tenantId, string userId, string token)
        {
            this.FirstName = firstName;
            this.UserId = userId;
            this.Email = email;
            this.TenantId = tenantId;
            this.Token = token;

        }
        public string FirstName { get; }
        public string UserId { get; }
        public string Email { get; }
        public string TenantId { get; }
        public string Token { get; }
    }
}