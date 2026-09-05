using CampusServicePortal.Modules.Notifications.Entities;

namespace CampusServicePortal.Modules.Notifications.Interfaces.Repository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);

        Task<Notification?> GetByIdAsync(int notificationId);

        Task<Notification> CreateAsync(Notification notification);

        Task<bool> MarkAsReadAsync(int notificationId);

        Task<bool> DeleteAsync(int notificationId);
    }
}