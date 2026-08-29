using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Entities;
using CampusServicePortal.Modules.Notifications.Interfaces.Repository;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;

namespace CampusServicePortal.Modules.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId)
        {
            var notifications =
                await _notificationRepository.GetByUserIdAsync(userId);

            return notifications.Select(notification => new NotificationDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                ReferenceType = notification.ReferenceType,
                ReferenceId = notification.ReferenceId,
                CreatedAt = notification.CreatedAt
            });
        }

        public async Task<NotificationDto?> GetByIdAsync(int notificationId)
        {
            var notification =
                await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return null;
            }

            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                ReferenceType = notification.ReferenceType,
                ReferenceId = notification.ReferenceId,
                CreatedAt = notification.CreatedAt
            };
        }

        public async Task<NotificationDto> CreateAsync(
            NotificationCreateDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                IsRead = false,
                ReferenceType = dto.ReferenceType,
                ReferenceId = dto.ReferenceId,
                CreatedAt = DateTime.UtcNow
            };

            var createdNotification =
                await _notificationRepository.CreateAsync(notification);

            return new NotificationDto
            {
                NotificationId = createdNotification.NotificationId,
                UserId = createdNotification.UserId,
                Title = createdNotification.Title,
                Message = createdNotification.Message,
                IsRead = createdNotification.IsRead,
                ReferenceType = createdNotification.ReferenceType,
                ReferenceId = createdNotification.ReferenceId,
                CreatedAt = createdNotification.CreatedAt
            };
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            return await _notificationRepository
                .MarkAsReadAsync(notificationId);
        }

        public async Task<bool> DeleteAsync(int notificationId)
        {
            return await _notificationRepository
                .DeleteAsync(notificationId);
        }
    }
}
