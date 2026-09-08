namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Enums
{
    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner
    }

    public enum OrderType
    {
        Pickup,
        Delivery
    }

    public enum CanteenOrderStatus
    {
        Pending,
        Preparing,
        Ready,
        OutForDelivery,
        Delivered,
        Collected,
        Cancelled
    }
}
