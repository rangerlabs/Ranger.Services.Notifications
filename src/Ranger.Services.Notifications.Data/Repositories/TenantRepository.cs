using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ranger.Services.Notifications.Data {
    public class NotificationsRepository : INotificationsRepository {
        private readonly NotificationsDbContext context;

        public NotificationsRepository (NotificationsDbContext context) {
            this.context = context;
        }
    }
}