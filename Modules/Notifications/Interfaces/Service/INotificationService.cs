using CampusServicePortal.Modules.Notifications.DTOs;

namespace CampusServicePortal.Modules.Notifications.Interfaces.Service
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);

        Task<NotificationDto?> GetByIdAsync(int notificationId);

        Task<NotificationDto> CreateAsync(NotificationCreateDto dto);

        Task<bool> MarkAsReadAsync(int notificationId);

        Task<bool> DeleteAsync(int notificationId);
    }
}