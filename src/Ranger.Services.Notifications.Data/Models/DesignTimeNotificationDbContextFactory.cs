using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Ranger.Services.Notifications.Data {
    public class DesignTimeNotificationsDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext> {
        public NotificationsDbContext CreateDbContext (string[] args) {
            var config = new ConfigurationBuilder ()
                .SetBasePath (System.IO.Directory.GetCurrentDirectory ())
                .AddJsonFile ("appsettings.json")
                .Build ();

            var options = new DbContextOptionsBuilder<NotificationsDbContext> ();
            options.UseNpgsql (config["cloudSql:ConnectionString"]);

            return new NotificationsDbContext (options.Options);
        }
    }
}