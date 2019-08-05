namespace Ranger.Services.Notifications.Data {
    public class DatabaseCredentials {
        public DatabaseCredentials (string username, string password) {
            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
    }
}