namespace CampusServicePortal.Modules.Notifications.DTOs
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public string ReferenceType { get; set; } = string.Empty;

        public int ReferenceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}