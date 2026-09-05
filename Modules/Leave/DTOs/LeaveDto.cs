namespace CampusServicePortal.Modules.Leave.DTOs
{
    public class LeaveDto
    {
        public int LeaveId { get; set; }

        public int UserId { get; set; }

        public string LeaveType { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}