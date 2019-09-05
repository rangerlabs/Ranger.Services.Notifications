using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ranger.Common;

namespace Ranger.Services.Notifications.Data
{
    public class NotificationsDbContextInitializer : INotificationsDbContextInitializer
    {
        private readonly NotificationsDbContext context;

        public NotificationsDbContextInitializer(NotificationsDbContext context)
        {
            this.context = context;
        }

        public bool EnsureCreated()
        {
            return context.Database.EnsureCreated();
        }

        public void Migrate()
        {
            context.Database.Migrate();
        }

        public void Seed()
        {
            var notifications = FrontendNotificationsConfig.GetFrontendNotifications();
            foreach (var notification in notifications)
            {
                var existingNotification = context.FrontendNotifications.FirstOrDefault(
                    fn => fn.BackendEventKey == notification.BackendEventKey &&
                    fn.OperationsState == notification.OperationsState
                    );

                if (existingNotification is null)
                {
                    context.Add(notification);
                }
            }
            context.SaveChanges();
        }
    }

    public interface INotificationsDbContextInitializer
    {
        bool EnsureCreated();
        void Migrate();
        void Seed();
    }
}