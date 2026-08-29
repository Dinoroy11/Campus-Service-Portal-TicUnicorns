namespace CampusServicePortal_TicUnicorns.Modules.Complaints.DTOs
{
    public class UpdateComplaintDto
    {
        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;
    }
}