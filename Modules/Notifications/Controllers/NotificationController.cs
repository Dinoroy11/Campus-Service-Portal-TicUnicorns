using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Notifications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByUserId(int userId)
        {
            var notifications = await _notificationService.GetByUserIdAsync(userId);

            return Ok(notifications);
        }

        [HttpGet("{notificationId}")]
        public async Task<ActionResult<NotificationDto>> GetById(int notificationId)
        {
            var notification = await _notificationService.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> Create(NotificationCreateDto dto)
        {
            var notification = await _notificationService.CreateAsync(dto);

            return Ok(notification);
        }

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> Delete(int notificationId)
        {
            var result = await _notificationService.DeleteAsync(notificationId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}