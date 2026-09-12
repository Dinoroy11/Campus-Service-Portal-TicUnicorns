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

        public async Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(
            int userId,
            bool unreadOnly = false)
        {
            ValidateUserId(userId);

            var notifications = await _notificationRepository
                .GetByUserIdAsync(userId, unreadOnly);

            return notifications.Select(MapToDto).ToList();
        }

        public async Task<NotificationDto?> GetMyNotificationByIdAsync(
            int notificationId,
            int userId)
        {
            ValidateUserId(userId);

            if (notificationId <= 0)
            {
                return null;
            }

            var notification = await _notificationRepository
                .GetByIdForUserAsync(notificationId, userId);

            return notification == null
                ? null
                : MapToDto(notification);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            ValidateUserId(userId);

            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task<NotificationDto> CreateAsync(NotificationCreateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            ValidateUserId(dto.UserId);

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Notification title is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                throw new ArgumentException("Notification message is required.");
            }

            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title.Trim(),
                Message = dto.Message.Trim(),
                IsRead = false,
                ReferenceType = string.IsNullOrWhiteSpace(dto.ReferenceType)
                    ? "General"
                    : dto.ReferenceType.Trim(),
                ReferenceId = dto.ReferenceId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _notificationRepository.CreateAsync(notification);

            return MapToDto(created);
        }

        public async Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId)
        {
            ValidateUserId(userId);

            if (notificationId <= 0)
            {
                return false;
            }

            return await _notificationRepository
                .MarkAsReadAsync(notificationId, userId);
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            ValidateUserId(userId);

            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task<bool> DeleteAsync(
            int notificationId,
            int userId)
        {
            ValidateUserId(userId);

            if (notificationId <= 0)
            {
                return false;
            }

            return await _notificationRepository
                .DeleteAsync(notificationId, userId);
        }

        private static NotificationDto MapToDto(Notification notification)
        {
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

        private static void ValidateUserId(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("A valid user id is required.");
            }
        }
    }
}
