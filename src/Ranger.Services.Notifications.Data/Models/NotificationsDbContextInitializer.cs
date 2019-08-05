using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ranger.Common;

namespace Ranger.Services.Notifications.Data {
    public class NotificationsDbContextInitializer : INotificationsDbContextInitializer {
        private readonly NotificationsDbContext context;

        public NotificationsDbContextInitializer (NotificationsDbContext context) {
            this.context = context;
        }

        public bool EnsureCreated () {
            return context.Database.EnsureCreated ();
        }

        public void Migrate () {
            context.Database.Migrate ();
        }
    }

    public interface INotificationsDbContextInitializer {
        bool EnsureCreated ();
        void Migrate ();
    }
}