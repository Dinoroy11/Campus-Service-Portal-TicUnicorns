namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Entities
{
    public class CanteenEntities
    {
        public int CanteenId { get; set; }

        public int StudentId { get; set; }

        public string FoodName { get; set; } = string.Empty;

        public string MealType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string OrderType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime OrderedAt { get; set; }

        public DateTime? ReadyAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime? CollectedAt { get; set; }
    }
}
