namespace CampusServicePortal_TicUnicorns.Modules.Complaints.DTOs
{
    public class ComplaintResponseDto
    {
        public int ComplaintId { get; set; }

        public int StudentId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public int? AssignedTo { get; set; }

        public string? Resolution { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}