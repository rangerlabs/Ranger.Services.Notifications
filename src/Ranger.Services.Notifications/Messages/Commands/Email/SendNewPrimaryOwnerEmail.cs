using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendNewPrimaryOwnerEmail : ICommand
    {
        public string Email { get; }
        public string FirstName { get; }
        public string TenantId { get; }
        public string Token { get; }

        public SendNewPrimaryOwnerEmail(string email, string firstName, string tenantId, string token)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new System.ArgumentException(nameof(email));
            }

            if (firstName is null)
            {
                throw new System.ArgumentNullException(nameof(firstName));
            }

            if (tenantId is null)
            {
                throw new System.ArgumentNullException(nameof(tenantId));
            }

            if (token is null)
            {
                throw new System.ArgumentNullException(nameof(token));
            }

            this.Email = email;
            this.FirstName = firstName;
            this.TenantId = tenantId;
            this.Token = token;
        }
    }
}