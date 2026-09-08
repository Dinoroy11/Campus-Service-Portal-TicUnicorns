using CampusServicePortal.Modules.Notifications.Entities;
using CampusServicePortal.Modules.Notifications.Interfaces.Repository;

namespace CampusServicePortal.Modules.Notifications.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly List<Notification> _notifications = new();

        public Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            var notifications = _notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Task.FromResult<IEnumerable<Notification>>(notifications);
        }

        public Task<Notification?> GetByIdAsync(int notificationId)
        {
            var notification = _notifications
                .FirstOrDefault(x => x.NotificationId == notificationId);

            return Task.FromResult(notification);
        }

        public Task<Notification> CreateAsync(Notification notification)
        {
            notification.NotificationId = _notifications.Count + 1;
            notification.CreatedAt = DateTime.UtcNow;

            _notifications.Add(notification);

            return Task.FromResult(notification);
        }

        public Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = _notifications
                .FirstOrDefault(x => x.NotificationId == notificationId);

            if (notification == null)
            {
                return Task.FromResult(false);
            }

            notification.IsRead = true;

            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int notificationId)
        {
            var notification = _notifications
                .FirstOrDefault(x => x.NotificationId == notificationId);

            if (notification == null)
            {
                return Task.FromResult(false);
            }

            _notifications.Remove(notification);

            return Task.FromResult(true);
        }
    }
}