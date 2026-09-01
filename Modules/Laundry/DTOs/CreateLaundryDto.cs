namespace CampusServicePortal_TicUnicorns.Modules.Laundry.DTOs
{
    public class CreateLaundryDto
    {
        public int StudentId { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string PickupMethod { get; set; } = string.Empty;
    }
}