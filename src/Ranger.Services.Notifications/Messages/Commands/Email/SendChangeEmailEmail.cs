using Ranger.RabbitMQ;

namespace Ranger.Services.Notifications
{
    [MessageNamespace("notifications")]
    public class SendChangeEmailEmail : ICommand
    {
        public SendChangeEmailEmail(string firstName, string email, string domain, string userId, string organization, string token)
        {
            this.FirstName = firstName;
            this.UserId = userId;
            this.Email = email;
            this.Domain = domain;
            this.Organization = organization;
            this.Token = token;

        }
        public string FirstName { get; }
        public string UserId { get; }
        public string Email { get; }
        public string Domain { get; }
        public string Organization { get; }
        public string Token { get; }
    }
}