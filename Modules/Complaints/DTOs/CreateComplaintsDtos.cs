namespace CampusServicePortal_TicUnicorns.Modules.Complaints.DTOs
{
    public class CreateComplaintDto
    {
        public int StudentId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Priority { get; set; } = "Medium";
    }
}