using CampusServicePortal.Modules.Notifications.Entities;
using CampusServicePortal.Modules.Notifications.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Notifications.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly CampusDbContext _context;

        public NotificationRepository(CampusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(
            int userId,
            bool unreadOnly = false)
        {
            var query = _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .ThenByDescending(n => n.NotificationId)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdForUserAsync(
            int notificationId,
            int userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == notificationId &&
                    n.UserId == userId);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .CountAsync(n =>
                    n.UserId == userId &&
                    !n.IsRead);
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            notification.IsRead = false;
            notification.CreatedAt = DateTime.UtcNow;

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == notificationId &&
                    n.UserId == userId);

            if (notification == null)
            {
                return false;
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n =>
                    n.UserId == userId &&
                    !n.IsRead)
                .ToListAsync();

            if (notifications.Count == 0)
            {
                return 0;
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return notifications.Count;
        }

        public async Task<bool> DeleteAsync(
            int notificationId,
            int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == notificationId &&
                    n.UserId == userId);

            if (notification == null)
            {
                return false;
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
