using CampusServicePortal.Modules.Notifications.DTOs;

namespace CampusServicePortal.Modules.Notifications.Interfaces.Service
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(
            int userId,
            bool unreadOnly = false);

        Task<NotificationDto?> GetMyNotificationByIdAsync(
            int notificationId,
            int userId);

        Task<int> GetUnreadCountAsync(int userId);

        // Used internally by other modules to generate notifications.
        Task<NotificationDto> CreateAsync(NotificationCreateDto dto);

        Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId);

        Task<int> MarkAllAsReadAsync(int userId);

        Task<bool> DeleteAsync(
            int notificationId,
            int userId);
    }
}
