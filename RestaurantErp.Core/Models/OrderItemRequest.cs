namespace RestaurantErp.Core
{
    public class OrderItemRequest
    {
        public Guid OrderId { get; set; }

        public int GuestNumber { get; set; }

        public DishEnum Dish { get; set; }

        public int Count { get; set; }
    }
}