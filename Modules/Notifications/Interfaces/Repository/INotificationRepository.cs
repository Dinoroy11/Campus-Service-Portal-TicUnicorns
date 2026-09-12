using CampusServicePortal.Modules.Notifications.Entities;

namespace CampusServicePortal.Modules.Notifications.Interfaces.Repository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(
            int userId,
            bool unreadOnly = false);

        Task<Notification?> GetByIdForUserAsync(
            int notificationId,
            int userId);

        Task<int> GetUnreadCountAsync(int userId);

        Task<Notification> CreateAsync(Notification notification);

        Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId);

        Task<int> MarkAllAsReadAsync(int userId);

        Task<bool> DeleteAsync(
            int notificationId,
            int userId);
    }
}
