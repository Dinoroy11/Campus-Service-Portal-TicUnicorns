using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusServicePortal.Modules.Notifications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notification/my
        // GET: api/Notification/my?unreadOnly=true
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyNotifications(
            [FromQuery] bool unreadOnly = false)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user identity." });
            }

            var notifications = await _notificationService
                .GetMyNotificationsAsync(userId, unreadOnly);

            return Ok(notifications);
        }

        // GET: api/Notification/my/unread-count
        [HttpGet("my/unread-count")]
        public async Task<IActionResult> GetMyUnreadCount()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user identity." });
            }

            var count = await _notificationService.GetUnreadCountAsync(userId);

            return Ok(new { unreadCount = count });
        }

        // GET: api/Notification/12
        // A user can read only his/her own notification.
        [HttpGet("{notificationId:int}")]
        public async Task<ActionResult<NotificationDto>> GetById(int notificationId)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user identity." });
            }

            var notification = await _notificationService
                .GetMyNotificationByIdAsync(notificationId, userId);

            if (notification == null)
            {
                return NotFound(new { message = "Notification not found." });
            }

            return Ok(notification);
        }

        // PUT: api/Notification/12/read
        // A user can mark only his/her own notification as read.
        [HttpPut("{notificationId:int}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user identity." });
            }

            var result = await _notificationService
                .MarkAsReadAsync(notificationId, userId);

            if (!result)
            {
                return NotFound(new { message = "Notification not found." });
            }

            return Ok(new { message = "Notification marked as read." });
        }

        // PUT: api/Notification/read-all
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user identity." });
            }

            var updatedCount = await _notificationService.MarkAllAsReadAsync(userId);

            return Ok(new
            {
                message = "All notifications marked as read.",
                updatedCount
            });
        }

        // DELETE: api/Notification/12
        // Optional cleanup endpoint. A user can delete only his/her own notification.
        [HttpDelete("{notificationId:int}")]
        public async Task<IActionResult> Delete(int notificationId)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user identity." });
            }

            var result = await _notificationService
                .DeleteAsync(notificationId, userId);

            if (!result)
            {
                return NotFound(new { message = "Notification not found." });
            }

            return Ok(new { message = "Notification deleted." });
        }

        // IMPORTANT:
        // There is intentionally NO public POST endpoint here.
        // Notifications are created internally by Hostel, Lab, Event,
        // Canteen, Gym, Certificate, Complaint, Leave, Sports, etc.

        private bool TryGetCurrentUserId(out int userId)
        {
            userId = 0;

            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out userId) && userId > 0;
        }
    }
}
