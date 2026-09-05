namespace CampusServicePortal_TicUnicorns.Modules.Complaints.DTOs
{
    public class UpdateComplaintStatusDto
    {
        public string Status { get; set; } = string.Empty;

        public string? Resolution { get; set; }
    }
}