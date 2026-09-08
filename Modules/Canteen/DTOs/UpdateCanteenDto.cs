namespace CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs
{
    public class UpdateCanteenDto
    {
        public string FoodName { get; set; } = string.Empty;

        public string MealType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string OrderType { get; set; } = string.Empty;
    }
}
