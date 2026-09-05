namespace CampusServicePortal_TicUnicorns.Modules.Laundry.DTOs
{
    public class LaundryResponseDto
    {
        public int LaundryId { get; set; }

        public int StudentId { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string PickupMethod { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime RequestedAt { get; set; }

        public DateTime? ReadyAt { get; set; }

        public int? AssignedTo { get; set; }

        public DateTime? CollectedAt { get; set; }
    }
}