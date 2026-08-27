namespace CampusServicePortal.Modules.Notifications.DTOs
{
    public class NotificationCreateDto
    {
        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string ReferenceType { get; set; } = string.Empty;

        public int ReferenceId { get; set; }
    }
}